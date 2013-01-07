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

namespace ProjectDream.MiniGames.Duel
{
    public class Duel_PlayerManager
    {
        Duel_Character[] characters;
        Player[] players;

        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;

        AudioEngine audioEngine;
        WaveBank waveBank; // has all our .wav files
        SoundBank soundBank; // collection of sounds

        const float MAX_SPEED = 7.0F;
        const float ACCELERATION = 1.0F;
        const float DAMPENER = 75;
        const float BUTTON_CHANGE = 7000;
        public Buttons currentButton;
        float buttonTimer = 0;
        Random random;

        int id1 = 0;
        int id2 = 0;

        public Duel_PlayerManager(Player[] players)
        {
            this.players = players;
            random = new Random();
            currentButton = Buttons.A;
        }

        public void LoadContent(ContentManager Content, Vector2 center)
        {
            characters = new Duel_Character[4];
            audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");

            waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");

            players[0].sprite = Content.Load<Texture2D>(@"Character\BlueKnightSpriteSheet");
            players[1].sprite = Content.Load<Texture2D>(@"Character\PinkKnightSpriteSheet");
            players[2].sprite = Content.Load<Texture2D>(@"Character\BlackKnightSpriteSheet");
            players[3].sprite = Content.Load<Texture2D>(@"Character\GreenKnightSpriteSheet");

            for (int i = 1; i < 4; i++)
            {
                if (players[i] != null )
                {
                    if (players[i].Score > players[id1].Score)
                    {
                        id2 = id1;
                        id1 = i;
                    }
                    else if (i == 1)
                    {
                        id2 = i;
                    }
                    else if (players[i].Score > players[id2].Score)
                    {
                        id2 = i;
                    }
                }
            }
            characters[id1] = new Duel_Character();
            characters[id1].Initialize(players[id1].sprite,
                                  center + new Vector2(400, 0),
                                  Vector2.Zero);
            characters[id1].team = 0;

            characters[id2] = new Duel_Character();
            characters[id2].Initialize(players[id2].sprite,
                                  center + new Vector2(-400, 0),
                                  Vector2.Zero);
            characters[id2].team = 1;
            characters[id2].effect = SpriteEffects.FlipHorizontally;

        }

        public void InitializePlayerPositions(Vector2 center)
        {
        }
        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
           // buttonTimer += elapsedTime;
            //Could Change to progress with game ( increment more when game is at the end )
            buttonTimer += random.Next(0, 100);

            if (buttonTimer >= BUTTON_CHANGE)
            {
                int button = 4096 * (int)Math.Pow(2, random.Next(0, 4));
                currentButton = (Buttons)button;
                buttonTimer = 0;
            }

            PlayerActions(elapsedTime);

            int counter = 0;
            foreach (Duel_Character player in characters)
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
            foreach (Duel_Character player in characters)
            {
                if (player != null)
                    player.Move(elapsedTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Duel_Character player in characters)
            {
                if (player != null)
                    player.Draw(spriteBatch);
            }
        }

        private void PlayerActions(float elapsedTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState(); 
            if (id1 == 0)
                PlayerInput(characters[id1], GamePad.GetState(PlayerIndex.One), Keys.K, Keys.I, Keys.J, Keys.L, elapsedTime, 0);
            if (id1 == 1)
                PlayerInput(characters[id1], GamePad.GetState(PlayerIndex.Two), Keys.G, Keys.T, Keys.F, Keys.H, elapsedTime, 0);
            if (id1 == 2)
                PlayerInput(characters[id1], GamePad.GetState(PlayerIndex.Three), Keys.W, Keys.S, Keys.D, Keys.A, elapsedTime, 0);
            if (id1 == 3)
                PlayerInput(characters[id1], GamePad.GetState(PlayerIndex.Four), Keys.Up, Keys.Down, Keys.Right, Keys.Left, elapsedTime, 0);
            if( id2 == 0)
                PlayerInput(characters[id2], GamePad.GetState(PlayerIndex.One), Keys.K, Keys.I, Keys.J, Keys.L, elapsedTime, 0);
            if (id2 == 1)
                PlayerInput(characters[id2], GamePad.GetState(PlayerIndex.Two), Keys.G, Keys.T, Keys.F, Keys.H, elapsedTime, 0);            
            if (id2 == 2)
                PlayerInput(characters[id2], GamePad.GetState(PlayerIndex.Three), Keys.W, Keys.S, Keys.D, Keys.A, elapsedTime, 0);
            if (id2 == 3)
                PlayerInput(characters[id2], GamePad.GetState(PlayerIndex.Four), Keys.Up, Keys.Down, Keys.Right, Keys.Left, elapsedTime, 0);
        }

        private void PlayerInput(Duel_Character player, GamePadState padState, Keys up, Keys down, Keys right, Keys left, float elapsedTime, int auto)
        {

            player.velocity = new Vector2(-ACCELERATION, 0);
            if (player.isInvulnerable)
                player.CurrentAction = Duel_Character.Action.Knockback;
            else if (player.dying)
            {
                //Dont allow input
            }
            else
            {
                if (player.velocity.Length() <= 4)
                {
                    if (currentKeyboardState.IsKeyDown(up) || padState.IsButtonDown(currentButton) || (auto == 1 && player.pushed == false))
                    {
                        if (!player.pushed)
                        {
                            //player.CurrentAction = Duel_Character.Action.Knockback;
                            player.ChangeFrames(1000.0f);
                            player.velocity = new Vector2(MAX_SPEED, 0);
                        }
                        player.pushed = true;
                    }
                    else
                    {
                        player.pushed = false;
                    }
                }
            }
        }

        public void CheckCollision(Duel_Character checkingPlayer, int counter)
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
                            characters[i].CurrentAction = Duel_Character.Action.Knockback;
                            checkingPlayer.CurrentAction = Duel_Character.Action.Knockback;
                        }
                    }
                }
            }
        }

        public int KillLoosers(Vector2 center, float scale)
        {
            //Check distance from center to center of player
            int deadCount = 0;
            foreach(Duel_Character Duel_Character in characters)
            {
                if (Duel_Character != null)
                {
                    float absDist = (float)Math.Abs(center.X - Duel_Character.position.X);
                    if (Duel_Character.dead)
                    {
                        deadCount++;
                        //Delete the Duel_Character, he is dead
                        Duel_Character.CurrentAction = Duel_Character.Action.Die;
                    }
                    else if (absDist <= scale)
                    {
                        deadCount++;
                        Duel_Character.dying = true;
                        Duel_Character.CurrentAction = Duel_Character.Action.Duck;
                    }
                }
            }
            return deadCount;
        }

        public void setWinner()
        {
            //CHECK IF THERE IS ONLY ONE PLAYER , OR time has elapsed
            // HE is the winner
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {        
                    if(characters[i].dead)
                    {
                        characters[i].CurrentAction = Duel_Character.Action.Duck;
                      
                    }
                    else
                    {
                        characters[i].velocity = new Vector2(0, 0);
                        characters[i].CurrentAction = Duel_Character.Action.Jump;
                        //Everyone who isn't dead get's a point.
                        if (!characters[i].dying)
                        {
                            players[i].Score += 4;
                        }
                    }
                }
                
            }
        }
    }
}