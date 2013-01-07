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
    class LumberjackBustle : MiniGame
    {
        const int NUM_PLAYERS = 4;
        const int START = 700;
        const int FINISH = 45000;

        const int START_TIMER = 3990;
        const int DIFFICULTY = 13; // Increasing this makes the game harder

        Texture2D background;
        Texture2D finish;
        Texture2D countdown;
        Texture2D blackHole;
        float holeRotation;
        Rectangle holeRect = new Rectangle(130, 0, 130, 130);
        Vector2 holeOrigin = new Vector2(65,65);

        int seconds;
        bool started;
        bool winnerIsSet = false;

        Camera camera;
        SpriteFont font;

        float timer;
        Lane[] lanes;
        PlayerManager playerManager;

        int currentObstacle;

        bool instructionsAreOn = true;
        LumberjackInstruction instruction;

        Song theme;
        Song victory;
        bool victorySongPlaying = false;

        public LumberjackBustle(Player[] players)
        {
            Players = players;
            camera = new Camera();
            instruction = new LumberjackInstruction();
            playerManager = new PlayerManager(Players);
        }

        public void Initialize()
        {
            base.Initialize(MiniGamePlayerType.FFA, "Lumberjack Bustle");
        }

        public override void LoadContent(ContentManager Content)
        {
            //theme = Content.Load<Song>(@"Lumberjack Bustle\BennyHillTheme");
            //victory = Content.Load<Song>(@"Lumberjack Bustle\ff4victory");

            instruction.Load(Content);

            background = Content.Load<Texture2D>("Lumberjack Bustle/forestBackground");
            background = Content.Load<Texture2D>("Lumberjack Bustle/ffBackground");
            countdown = Content.Load<Texture2D>("Lumberjack Bustle/Countdown");
            finish = Content.Load<Texture2D>("Lumberjack Bustle/finishline");
            blackHole = Content.Load<Texture2D>("Lumberjack Bustle/blackHole");

            //countdown = Content.Load<Texture2D>("Lumberjack Bustle/treeBackground.png");
            font = Content.Load<SpriteFont>("Score");
            seconds = 3;
            started = false;

            lanes = new Lane[4];
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                lanes[i] = new Lane();
                lanes[i].Initialize(new Vector2(0, i * Lane.HEIGHT), background, finish);
            }

            InitializeLanes(Content);

            playerManager.LoadContent(Content, START);
            currentObstacle = 0;
        }

        public void InitializeLanes(ContentManager Content)
        {
            ObstacleFactory oFactory = new ObstacleFactory(Content);
            int initialObstacleInterval = 130 * DIFFICULTY;
            int minInterval = 350;

            int obstacleInterval = initialObstacleInterval;
            float randomConstant = (initialObstacleInterval - minInterval);

            Random random = new Random();

            for (int i = initialObstacleInterval + START; i < FINISH; i += obstacleInterval)
            {
                //Generate a random TYPE 
                ObstacleType type = oFactory.GetRandomType();
                float progressPercent = (float)(i - START) / (float)FINISH;
                obstacleInterval = initialObstacleInterval - (int)(progressPercent * randomConstant);

                if (obstacleInterval < minInterval)
                    obstacleInterval = minInterval;

                obstacleInterval = random.Next(minInterval, obstacleInterval );

                for (int lane = 0; lane < NUM_PLAYERS; lane++)
                {
                    //Make an object based on generateed TYPE for each Lane                    
                    Obstacle newObstacle = oFactory.GetObstacleByType(type, new Vector2(i, lane * Lane.HEIGHT));
                    lanes[lane].AddObstacle(newObstacle);
                }
            }

            playerManager.LoadContent(Content, START);
        }

        public override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed ||
            //    Keyboard.GetState().IsKeyDown(Keys.Enter))
            //{
            //    GameOver = true;
            //}

            if (instructionsAreOn)
            {
                instruction.Update();
                if (instruction.ReadyToStart())
                {
                    instructionsAreOn = false;
                   // MediaPlayer.Play(theme);
                }

            }
            else
            {
                if (!started)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    seconds = 3 - (int)timer / 1000;
                }
                if (timer > START_TIMER)
                {
                    started = true;
                    holeRotation += 0.5f;

                    MoveCamera(gameTime);

                    foreach (Lane lane in lanes)
                    {
                        if (lane != null)
                        {
                            if (lane.obstacles.Count > 0)
                            {
                                if (lane.obstacles[currentObstacle].worldPosition.X + 200 <= camera.Position.X)
                                {
                                    lane.obstacles[currentObstacle].IsAlive = false;
                                    lane.obstacles[currentObstacle].IsReadyForDelete = true;
                                }
                            }
                        }

                        playerManager.CheckCollision(lanes, camera, currentObstacle);
                        int deadCount = playerManager.KillLoosers();
                        if (deadCount == 4)
                        {
                            GameOver = true;
                        }
                    }
                }
                
                foreach (Lane lane in lanes)
                {
                    lane.Update(gameTime);
                }
            }
        }

        private void MoveCamera(GameTime gameTime)
        {
            // make keyboard move the camera
            KeyboardState ks = Keyboard.GetState();
            Keys[] keys = ks.GetPressedKeys();
            float speed = DIFFICULTY;

            if (camera.Position.X >= FINISH + 200)
            {
                //GAME OVER
                if (!winnerIsSet)
                {
                    playerManager.setWinner();
                    winnerIsSet = true;

                }
                playerManager.UpdateNoInput(gameTime);

                if (!victorySongPlaying)
                {
                    victorySongPlaying = true;
                   // MediaPlayer.Play(victory);
                }

                foreach (Keys key in keys)
                {
                    switch (key)
                    {
                        case Keys.Enter:
                            GameOver = true;
                            break;
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                {
                    GameOver = true;
                }
            }
            else
            {
                camera.Translate(new Vector2(speed, 0));
                playerManager.Update(gameTime, camera.Position.X);
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
                for (int lane = 0; lane < NUM_PLAYERS; lane++)
                {
                    lanes[lane].Draw(spriteBatch, camera);
                    spriteBatch.Draw(blackHole, new Vector2(10, (lane * 180) + 20) + holeOrigin, holeRect, Color.White, holeRotation, holeOrigin, 1.0f, SpriteEffects.None, 0);
                }
                playerManager.Draw(spriteBatch);

                if (!started)
                {
                    Vector2 counterPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2 - countdown.Width / 2, Graphics.PreferredBackBufferHeight / 2 - countdown.Height / 2);
                    spriteBatch.Draw(countdown, counterPosition, Color.White);
                    spriteBatch.DrawString(font, seconds.ToString(), counterPosition + new Vector2(28, 7), Color.White, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                }
            }
        }

    }
}
