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

namespace ProjectDream.MiniGames.FireballTactics
{
    class FireballTactics : MiniGame
    {
        const int NUM_PLAYERS = 4;
        const int START_TIMER = 3990;
        const int GAME_TIMER = 30990;
        FT_PlayerManager playerManager;
        Texture2D background;
        Texture2D platform;        
        Texture2D ground;

        SpriteFont font;
        Texture2D countdown;

        float timer;
        float gameTimer;        
        float elevatedHeight;        
        int seconds;
        bool started;
        bool winnerIsSet = false;

        bool instructionsAreOn = true;
        FT_Instruction instruction;

        Song music;
        Song victory;
        bool victorySongPlaying = false;


        public FireballTactics(Player[] players)
        {
            Players = players;
            playerManager = new FT_PlayerManager(Players);
            instruction = new FT_Instruction();
        }

        public void Initialize()
        {
            base.Initialize(MiniGamePlayerType.ThreeVOne, "Fireball Tactics");
        }

        public override void LoadContent(ContentManager Content)
        {
            elevatedHeight = 50.0f;

            Fireball.groundLevel = Graphics.PreferredBackBufferHeight - (32.0f + elevatedHeight);
            FT_Character.groundLevel = Graphics.PreferredBackBufferHeight - (64.0f + elevatedHeight);
            FT_PlayerManager.fireballMeterPosition = Graphics.PreferredBackBufferWidth - 60.0F;

            //victory = Content.Load<Song>(@"Lumberjack Bustle\ff4victory");


            instruction.Load(Content);

            background = Content.Load<Texture2D>("FireballTactics/Fire");            
            ground = Content.Load<Texture2D>("FireballTactics/Ground");
            platform = Content.Load<Texture2D>("FireballTactics/Platform");
            font = Content.Load<SpriteFont>("Score");
            countdown = Content.Load<Texture2D>("Lumberjack Bustle/Countdown");
            //music = Content.Load<Song>(@"Lumberjack Bustle\BennyHillTheme");

            playerManager.LoadContent(Content, new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2));
            seconds = 3;
            started = false;            
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed ||
            // Keyboard.GetState().IsKeyDown(Keys.Enter))
            //{
            //    GameOver = true;
            //}

            if (instructionsAreOn)
            {
                instruction.Update();
                if (instruction.ReadyToStart())
                {
                    //MediaPlayer.Play(music);
                    instructionsAreOn = false;
                }

            }
            else
            {
                if (!started)
                {
                    timer += elapsedTime;
                    seconds = START_TIMER / 1000 - (int)timer / 1000;
                }
                if (timer > START_TIMER)
                {
                    started = true;
                    int deadCount = playerManager.FindEliminated();
                    seconds = GAME_TIMER / 1000 - (int)gameTimer / 1000;
                    if (gameTimer > GAME_TIMER - 60 || deadCount == 3)
                    {

                        if (!winnerIsSet)
                        {
                            playerManager.setWinner();
                            winnerIsSet = true;
                        }

                        playerManager.UpdateNoInput(gameTime);
                        if (!victorySongPlaying)
                        {
                            victorySongPlaying = true;
                            //MediaPlayer.Play(victory);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) ||
                            GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                        {
                            Firewall.firewallTimer = 0.0f;
                            GameOver = true;
                        }
                    }
                    else
                    {
                        gameTimer += elapsedTime;
                        playerManager.Update(gameTime);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (instructionsAreOn)
            {
                instruction.Draw(spriteBatch);
            }
            else
            {
                Rectangle backgroundRectangle = new Rectangle(0, 0, (int)Graphics.PreferredBackBufferWidth, (int)Graphics.PreferredBackBufferHeight);
                spriteBatch.Draw(background, backgroundRectangle, Color.White);

                Rectangle platformRectangle = new Rectangle(0, (int)(Graphics.PreferredBackBufferHeight / 2 - 206), (int)Graphics.PreferredBackBufferWidth, 10);
                spriteBatch.Draw(platform, platformRectangle, Color.White);

                playerManager.Draw(spriteBatch); 

                Rectangle groundRectangle = new Rectangle(0, (int)(Graphics.PreferredBackBufferHeight - elevatedHeight), (int)Graphics.PreferredBackBufferWidth, (int)elevatedHeight);
                spriteBatch.Draw(ground, groundRectangle, Color.White);                

                if (!started)
                {
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2 - countdown.Width / 2, Graphics.PreferredBackBufferHeight / 2 - countdown.Height / 2);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(28, 7), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 counterPosition = new Vector2(35.0f, 35.0f);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    if (seconds < 10)
                    {
                        spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(31, 12), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(16, 12), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }
        }

    }
}
