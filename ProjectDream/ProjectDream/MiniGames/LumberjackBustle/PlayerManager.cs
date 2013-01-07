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

namespace ProjectDream.MiniGames.LumberjackBustle
{
    public class PlayerManager
    {
        Character[] characters;
        Player[] players;

        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;

        AudioEngine audioEngine;
        WaveBank waveBank; // has all our .wav files
        SoundBank soundBank; // collection of sounds

        private float soundTimer = 0.0f;
        private float soundInterval = 100.0f;

        public PlayerManager(Player[] players)
        {
            this.players = players;
        }

        public void LoadContent(ContentManager Content, int Start)
        {
            
            characters = new Character[4];

            audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");

            waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");

            characters[0] = new Character();
            characters[0].Initialize(Content.Load<Texture2D>(@"Character\BlueKnightSpriteSheet"),
                                  new Vector2(Start, 170 - 64),
                                  Vector2.Zero);
            characters[1] = new Character();
            characters[1].Initialize(Content.Load<Texture2D>(@"Character\PinkKnightSpriteSheet"),
                                  new Vector2(Start, 350 - 64),
                                  Vector2.Zero);
            characters[2] = new Character();
            characters[2].Initialize(Content.Load<Texture2D>(@"Character\BlackKnightSpriteSheet"),
                                  new Vector2(Start, 530 - 64),
                                  Vector2.Zero);
            characters[3] = new Character();
            characters[3].Initialize(Content.Load<Texture2D>(@"Character\GreenKnightSpriteSheet"),
                                  new Vector2(Start, 710 - 64),
                                  Vector2.Zero);
        }

        public void Update(GameTime gameTime, float cameraXOffset)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            PlayerActions(elapsedTime);            
            foreach (Character player in characters)
            {
                if (player != null)
                {
                    player.Move(elapsedTime);
                }
            }
        }
        public void UpdateNoInput(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            foreach (Character player in characters)
            {
                if (player != null)
                    player.Move(elapsedTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Character player in characters)
            {
                if (player != null)
                    player.Draw(spriteBatch);
            }
        }

        private void PlayerActions(float elapsedTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            PlayerInput(characters[0], GamePad.GetState(PlayerIndex.One), Keys.Q, Keys.E, Keys.R, elapsedTime);
            PlayerInput(characters[1], GamePad.GetState(PlayerIndex.Two), Keys.U, Keys.J, Keys.K, elapsedTime);
            PlayerInput(characters[2], GamePad.GetState(PlayerIndex.Three), Keys.W, Keys.S, Keys.D, elapsedTime);
            PlayerInput(characters[3], GamePad.GetState(PlayerIndex.Four), Keys.Up, Keys.Down, Keys.Right, elapsedTime);
        }
        
        private void PlayerInput(Character player, GamePadState padState, Keys jump, Keys slide, Keys slash, float elapsedTime)
        {
            if (player.dying || player.dead)
            {
                //Nothing
                return;
            }
            else if (player.isInvulnerable)
                player.CurrentAction = Character.Action.Knockback;
            else if (currentKeyboardState.IsKeyDown(jump) || padState.IsButtonDown(Buttons.A))
            {
                player.CurrentAction = Character.Action.Jump;
                if (player.onGround)
                {
                    soundBank.PlayCue("Jump");
                }
            }
            else if (currentKeyboardState.IsKeyDown(slide) || padState.IsButtonDown(Buttons.DPadDown))
            {
                if (player.CurrentAction != Character.Action.Slide)
                {
                    soundBank.PlayCue("Slide");
                }

                player.CurrentAction = Character.Action.Slide;
            }
            else if (currentKeyboardState.IsKeyDown(slash) || padState.IsButtonDown(Buttons.X))
            {
                player.CurrentAction = Character.Action.Slash;
                soundTimer += elapsedTime;
                if (soundTimer > soundInterval)
                {
                    soundTimer = 0.0f;
                    soundBank.PlayCue("Slash");
                }
            }
            else
                player.CurrentAction = Character.Action.Run;
        }

        public void CheckCollision(Lane[] lanes, Camera camera, int current)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null && !characters[i].isInvulnerable)
                {
                    int checkingDepth = 10;
                    int limit = lanes[i].obstacles.Count;
                    if (limit > checkingDepth)
                        limit = checkingDepth;

                    for (int j = 0; j < limit; j++)
                    {
                        Obstacle obstacle = lanes[i].obstacles[current + j];
                        if (obstacle.Collides(characters[i].position, characters[i].collisionSize, camera.Position.X))
                        {
                            if (obstacle.Type == ObstacleType.Breakable &&
                                characters[i].CurrentAction == Character.Action.Slash)
                            {
                                var breakable = (BreakableObstacle)obstacle;
                                breakable.IsExploding = true;
                            }
                            else
                            {
                                characters[i].CurrentAction = Character.Action.Knockback;
                                soundBank.PlayCue("Collision");
                            }
                        }
                    }
                }
            }
        }

        public int KillLoosers()
        {
            int deadCount = 0;
            foreach(Character character in characters)
            {
                if (character != null)
                {
                    if(character.position.X <= 100)
                        character.dying = true;
                    if (character.dead)
                    {
                        deadCount++;
                        //Delete the character, he is dead
                        character.CurrentAction = Character.Action.Die;
                    }
                    //Counte deaths .. if == 4 end game
                }
            }
            return deadCount;
        }

        public void setWinner()
        {
            float max = 0; 
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    if (characters[i].position.X > max)
                        max = characters[i].position.X;
                }
            }
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    if (characters[i].position.X == max)
                    {
                        characters[i].CurrentAction = Character.Action.Jump;
                        //winner gets 2 points.
                        if (!characters[i].dead || !characters[i].dying)
                        {
                            players[i].Score += 2;
                        }
                    }
                    else
                    {
                        characters[i].CurrentAction = Character.Action.Slide;
                        if (!characters[i].dead || !characters[i].dying)
                        {
                            //Everyone who isn't dead get's a point.
                            players[i].Score += 1;
                        }
                    }

                }
                
            }
        }

    }
}
