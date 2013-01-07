using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectDream.MiniGames.BumperBalls
{
    public class BB_PlayerManager
    {
        BB_Character[] characters;
        Player[] players;

        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;

        AudioEngine audioEngine;
        WaveBank waveBank; // has all our .wav files
        SoundBank soundBank; // collection of sounds

        const float MAX_SPEED = 4;
        const float ACCELERATION = 0.1F;
        const float DAMPENER = 75;

        int id1 = 0;
        int id2 = 0;

        public BB_PlayerManager(Player[] players)
        {
            this.players = players;
        }

        public void LoadContent(ContentManager Content, Vector2 center)
        {
            characters = new BB_Character[4];
            audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");

            waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");

            players[0].sprite = Content.Load<Texture2D>(@"Character\BlueKnightSpriteSheet");
            players[1].sprite = Content.Load<Texture2D>(@"Character\PinkKnightSpriteSheet");
            players[2].sprite = Content.Load<Texture2D>(@"Character\BlackKnightSpriteSheet");
            players[3].sprite = Content.Load<Texture2D>(@"Character\GreenKnightSpriteSheet");
            characters[0] = new BB_Character();
            characters[1] = new BB_Character();
            characters[2] = new BB_Character();
            characters[3] = new BB_Character();

            int count1 = 1;
            int count2 = 1;
            for (int i = 1; i < 4; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Score > players[id1].Score)
                    {
                        id1 = i;
                    }
                    else if (players[i].Score < players[id2].Score)
                    {
                        id2 = i;
                    }
                    if (id1 == id2)
                    {
                        id2++;
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (i == id1 || i == id2)
                {
                    characters[i].Initialize(players[i].sprite,
                                    center + new Vector2(count1 * 100, -100),
                                    Vector2.Zero);
                    characters[i].team = 0;
                    count1 = -1;
                }
                else
                {
                    characters[i].Initialize(players[i].sprite,
                        center + new Vector2(count2 * 100, 100),
                        Vector2.Zero);
                    characters[i].team = 1;
                    count2 = -1;
                }
            }
        }

        public void InitializePlayerPositions(Vector2 center)
        {
        }
        public void Update(GameTime gameTime)
        {

            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            PlayerActions(elapsedTime);

            int counter = 0;
            foreach (BB_Character player in characters)
            {
                if (player != null && !player.dead)
                {
                    CheckCollision(player, counter);
                    player.Move(elapsedTime);
                }
                counter++;
            }
        }
        public void UpdateNoInput(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            foreach (BB_Character player in characters)
            {
                if (player != null)
                    player.Move(elapsedTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BB_Character player in characters)
            {
                if (player != null)
                    player.Draw(spriteBatch);
            }
        }

        private void PlayerActions(float elapsedTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            PlayerInput(characters[0], GamePad.GetState(PlayerIndex.One), Keys.Q, Keys.E, Keys.T, Keys.R, elapsedTime);
            PlayerInput(characters[1], GamePad.GetState(PlayerIndex.Two), Keys.Q, Keys.E, Keys.T, Keys.R, elapsedTime);
            PlayerInput(characters[2], GamePad.GetState(PlayerIndex.Three), Keys.W, Keys.S, Keys.D, Keys.A, elapsedTime);
            PlayerInput(characters[3], GamePad.GetState(PlayerIndex.Four), Keys.Up, Keys.Down, Keys.Right, Keys.Left, elapsedTime);
        }

        private void PlayerInput(BB_Character player, GamePadState padState, Keys up, Keys down, Keys right, Keys left, float elapsedTime)
        {
            if (player.isInvulnerable)
                player.CurrentAction = BB_Character.Action.Knockback;
            else if (player.dying)
            {
                //Dont allow input
            }
            else
            {
                if (player.velocity.Length() <= 4)
                {
                    //player.velocity = new Vector2(0, 0);
                    if (currentKeyboardState.IsKeyDown(left) || padState.IsButtonDown(Buttons.DPadLeft))
                    {
                        player.velocity += new Vector2(-ACCELERATION, 0);
                    }
                    if (currentKeyboardState.IsKeyDown(down) || padState.IsButtonDown(Buttons.DPadDown))
                    {
                        player.velocity += new Vector2(0, ACCELERATION);
                    }
                    if (currentKeyboardState.IsKeyDown(right) || padState.IsButtonDown(Buttons.DPadRight))
                    {
                        player.velocity += new Vector2(ACCELERATION, 0);
                    }
                    if (currentKeyboardState.IsKeyDown(up) || padState.IsButtonDown(Buttons.DPadUp))
                    {
                        player.velocity += new Vector2(0, -ACCELERATION);

                    }
                }

                //Check for max speed and normalize
                if (player.velocity.Length() >= MAX_SPEED)
                {
                    player.velocity.Normalize();
                    player.velocity = new Vector2(player.velocity.X * MAX_SPEED, player.velocity.Y * MAX_SPEED);
                }

                //Will check for no actiona and dampen out the speed
                if (!(currentKeyboardState.IsKeyDown(up) || padState.IsButtonDown(Buttons.DPadUp) ||
                    currentKeyboardState.IsKeyDown(right) || padState.IsButtonDown(Buttons.DPadRight) ||
                    currentKeyboardState.IsKeyDown(down) || padState.IsButtonDown(Buttons.DPadDown) ||
                    currentKeyboardState.IsKeyDown(left) || padState.IsButtonDown(Buttons.DPadLeft)))
                {
                    if (player.velocity.Length() != 0.0)
                    {
                        player.velocity += -player.velocity / DAMPENER;
                    }
                }
                player.CurrentAction = BB_Character.Action.Run;
            }
        }

        public void CheckCollision(BB_Character checkingPlayer, int counter)
        {
            
            for (int i = 0; i < characters.Length; i++)
            {
                if (counter != i)
                {
                    if (characters[i] != null && !characters[i].isInvulnerable)
                    {
                        if (checkingPlayer.collides(characters[i]))
                        {
                            //if (checkingPlayer.velocity == Vector2.Zero)
                            Vector2 knockDir = characters[i].position - checkingPlayer.position;
                            knockDir.Normalize();

                            checkingPlayer.knockDirection =  -1 * characters[i].velocity.Length() * knockDir;
                            characters[i].knockDirection = checkingPlayer.velocity.Length() * knockDir;

                            soundBank.PlayCue("Collision");
                            characters[i].CurrentAction = BB_Character.Action.Knockback;
                            checkingPlayer.CurrentAction = BB_Character.Action.Knockback;
                        }
                    }
                }
            }
        }

        public int KillLoosers(Vector2 center, float scale)
        {
            //Check distance from center to center of player

            int deadCount = 0;
            foreach(BB_Character BB_Character in characters)
            {
                if (BB_Character != null)
                {
                    Vector2 centerToPlayer = center - BB_Character.position;
                    Vector2 sizeOffset = new Vector2((centerToPlayer.X / Math.Abs(centerToPlayer.X )) * BB_Character.size.X / 2, 
                                                        (centerToPlayer.Y / Math.Abs(centerToPlayer.Y)) * BB_Character.size.Y / 2);

                    if (Vector2.Distance(BB_Character.position + sizeOffset, center) >= 325 * scale)
                        BB_Character.dying = true;
                    if (BB_Character.dead)
                    {
                        deadCount++;
                        //Delete the BB_Character, he is dead
                        BB_Character.CurrentAction = BB_Character.Action.Die;
                    }
                    //Counte deaths .. if == 4 end game
                }
            }
            return deadCount;
        }

        public int teamWin()
        {
            int deadCount1 = 0;
            int deadCount2 = 0;

            foreach (BB_Character BB_Character in characters)
            {
                if (BB_Character != null)
                {
                    if (BB_Character.dead)
                    {
                        if (BB_Character.team == 0)
                        {
                            deadCount1++;
                        }
                        else
                        {
                            deadCount2++;
                        }
                    }
                }
            }
            if (deadCount1 + deadCount2 != 4)
            {
                if (deadCount1 == 2)
                {
                    return 2;
                }

                if (deadCount2 == 2)
                {
                    return 1;
                }
            }
            return 0;
        }

        public void setWinner()
        {
            //CHECK IF THERE IS ONLY ONE PLAYER , OR time has elapsed
            // HE is the winner
            int winningTeam = 2;
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {        
                    if(characters[i].dead)
                    {
                        characters[i].CurrentAction = BB_Character.Action.Jump;
                    }
                    else
                    {
                        characters[i].velocity = new Vector2(0, 0);
                        characters[i].CurrentAction = BB_Character.Action.Slash;
                        if (!characters[i].dying)
                        {
                            winningTeam = characters[i].team;
                            
                        }
                    }
                }
            }
            for (int j = 0; j < characters.Length; j++)
            {
                if (characters[j].team == winningTeam)
                {
                    players[j].Score += 1;
                }
            } 
        } 
    }
}