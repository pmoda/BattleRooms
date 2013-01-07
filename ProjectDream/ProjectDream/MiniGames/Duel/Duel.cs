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
    class Duel : MiniGame
    {
        const int NUM_PLAYERS = 4;
        const int START_TIMER = 3990;
        const int GAME_TIMER = 60990;
        Duel_PlayerManager playerManager;
        Texture2D background;
        Texture2D pit;
        SpriteFont font;

        Texture2D countdown;

        float holeRotation;
        float timer;
        float gameTimer;
        int seconds;
        bool started;
        bool winnerIsSet = false;

        bool instructionsAreOn = true;
        Duel_Instruction instruction;

        Song music;
        Song victory;
        bool victorySongPlaying = false;

        public Duel(Player[] players)
        {
            Players = players;
            playerManager = new Duel_PlayerManager(Players);
            instruction = new Duel_Instruction();
        }
        
        public void Initialize()
        {
            base.Initialize(MiniGamePlayerType.FFA, "Duel");
        }

        public override void LoadContent(ContentManager Content)
        {

            //victory = Content.Load<Song>(@"Lumberjack Bustle\ff4victory");

            instruction.Load(Content);

            font = Content.Load<SpriteFont>("Score");
            background = Content.Load<Texture2D>("Duel/Sand");
            countdown = Content.Load<Texture2D>("Lumberjack Bustle/Countdown");
            //music = Content.Load<Song>(@"Lumberjack Bustle\BennyHillTheme");
            pit = Content.Load<Texture2D>("Lumberjack Bustle/blackHole");

            playerManager.LoadContent(Content, new Vector2(Graphics.PreferredBackBufferWidth/2, Graphics.PreferredBackBufferHeight/2));
            seconds = 3;
            started = false;
        }

        public void InitializeLanes(ContentManager Content)
        {
                        
        }

        public override void Update(GameTime gameTime)
        {
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
                    seconds = GAME_TIMER / 1000 - (int)gameTimer / 1000;
                    int deadCount = playerManager.KillLoosers(new Vector2(Graphics.PreferredBackBufferWidth / 2, 
                                                                Graphics.PreferredBackBufferHeight / 2), 50);
                   
                    if ( deadCount != 0)
                    {
                        playerManager.UpdateNoInput(gameTime);
                        if (!victorySongPlaying)
                        {
                            if (!winnerIsSet)
                            {
                                playerManager.setWinner();
                                winnerIsSet = true;
                            }

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
                Vector2 holePos = new Vector2(Graphics.PreferredBackBufferWidth/2 , Graphics.PreferredBackBufferHeight/2);
                Rectangle holeRect = new Rectangle(130, 0, 140, 128);
                Vector2 holeOrigin = new Vector2(70, 64);
                holeRotation += 0.005f;
                spriteBatch.Draw(background, Vector2.Zero, Color.White);
                spriteBatch.Draw(pit, holePos, holeRect, Color.FromNonPremultiplied(235, 235, 220, 20), holeRotation, holeOrigin, 7f, SpriteEffects.None, 0);
                spriteBatch.Draw(pit, holePos, holeRect, Color.FromNonPremultiplied(235, 235, 220, 50), holeRotation, holeOrigin, 1f, SpriteEffects.None, 0);
               
                spriteBatch.Draw(background, Vector2.Zero, Color.FromNonPremultiplied(255,255,255,50));
              //  spriteBatch.Draw(background, new Vector2(10,10), new Color(255,255,255,36));
           
                playerManager.Draw(spriteBatch);
                if (!started)
                {
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2 - countdown.Width / 2, Graphics.PreferredBackBufferHeight / 2 - countdown.Height / 2);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(28, 7), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 counterPosition1 = new Vector2(Graphics.PreferredBackBufferWidth / 2 - 24, Graphics.PreferredBackBufferHeight / 2 - 36);
                    spriteBatch.DrawString(font, playerManager.currentButton.ToString(), counterPosition1, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    /*
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth - 200, Graphics.PreferredBackBufferHeight - 200);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    if (seconds < 10)
                    {
                        spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(31, 12), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(16, 12), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }*/
                }
            }
        }

    }
}
