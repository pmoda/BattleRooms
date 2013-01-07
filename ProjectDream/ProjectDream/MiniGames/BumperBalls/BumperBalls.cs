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
    class BumperBalls : MiniGame
    {
        const int NUM_PLAYERS = 4;
        const int START_TIMER = 3990;
        const int GAME_TIMER = 60990;
        BB_PlayerManager playerManager;
        Texture2D background;
        Texture2D platform;
        SpriteFont font;

        Texture2D countdown;

        float timer;
        float gameTimer;
        int seconds;
        bool started;
        bool winnerIsSet = false;
        float plateformScale;

        bool instructionsAreOn = true;
        BB_Instruction instruction;

        Song music;
        Song victory;
        bool victorySongPlaying = false;

        public BumperBalls(Player[] players)
        {
            Players = players;
            playerManager = new BB_PlayerManager(Players);
            instruction = new BB_Instruction();
        }
        
        public void Initialize()
        {
            base.Initialize(MiniGamePlayerType.FFA, "Bumper Balls");
        }

        public override void LoadContent(ContentManager Content)
        {

            //victory = Content.Load<Song>(@"Lumberjack Bustle\ff4victory");

            instruction.Load(Content);

            background = Content.Load<Texture2D>("BumperBalls/Water");
            platform = Content.Load<Texture2D>("BumperBalls/iceplateform");
            font = Content.Load<SpriteFont>("Score");
            countdown = Content.Load<Texture2D>("Lumberjack Bustle/Countdown");
            //music = Content.Load<Song>(@"Lumberjack Bustle\BennyHillTheme"); 
            
            playerManager.LoadContent(Content, new Vector2(Graphics.PreferredBackBufferWidth/2, Graphics.PreferredBackBufferHeight/2));
            seconds = 3;
            started = false;
            plateformScale = 1;
        }

        public void InitializeLanes(ContentManager Content)
        {
                        
        }

        public override void Update(GameTime gameTime)
        {
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
                    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    seconds = START_TIMER/1000 - (int)timer / 1000;
                }
                if (timer > START_TIMER)
                {
                    started = true;
                    int deadCount = playerManager.KillLoosers(new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2), plateformScale);
                    int winningTeam = playerManager.teamWin();
                    seconds = GAME_TIMER / 1000 - (int)gameTimer / 1000;
                    if (gameTimer > GAME_TIMER-60 || deadCount == 3 || winningTeam != 0)
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
                            GameOver = true;
                        }
                    }
                    else
                    {
                        gameTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        playerManager.Update(gameTime);
                        plateformScale = 0.5f + (1 - gameTimer / GAME_TIMER) * 0.5f;
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
                spriteBatch.Draw(background, Vector2.Zero, Color.White);
                spriteBatch.Draw(platform, new Vector2(Graphics.PreferredBackBufferWidth / 2 - (platform.Width * plateformScale)/ 2, 
                    Graphics.PreferredBackBufferHeight / 2 - (platform.Height * plateformScale)/ 2), null, Color.White, 0, Vector2.Zero,
                    plateformScale, SpriteEffects.None, 0);
              //  spriteBatch.DrawString(font, "Bumper Balls - Under Construction!", new Vector2(250, 250), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

                playerManager.Draw(spriteBatch);
                if (!started)
                {
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2 - countdown.Width / 2, Graphics.PreferredBackBufferHeight / 2 - countdown.Height / 2);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    spriteBatch.DrawString(font, "TEAM 1", counterPosition + new Vector2(-75, -200), Color.Black);
                    spriteBatch.DrawString(font, "TEAM 2", counterPosition + new Vector2(-75, 200), Color.Black);
                    spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(28, 7), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth - 200, Graphics.PreferredBackBufferHeight - 200);
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
