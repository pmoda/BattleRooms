using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectDream.Tables;

namespace ProjectDream
{
    class Score
    {
        private const int screenWidth = 1280;
        private const int podiumStart = 515;
        private const int timeInterval = 500;
        private const int arial20CharacterSpacing = 13;
        private const int arial50CharacterSpacing = 33;

        private Player[] players;
        private bool isPlayer1Ready;
        private bool isPlayer2Ready;
        private bool isPlayer3Ready;
        private bool isPlayer4Ready;

        private Rectangle backgroundSourceRect;
        private Texture2D background;

        private Rectangle podiumSourceRect;
        private Texture2D podium;

        private bool pointsAreDone;
        private int numberOfPointsShowing = 0;
        private float pointCounter = 0;
        private List<Vector2> points;
        private Rectangle pointSourceRect;
        private Texture2D pointBar;

        private SpriteFont font;
        private SpriteFont titleFont;

        private Texture2D miniController;
        private Rectangle miniControlSourceRect;
        private Texture2D check;
        private Rectangle checkSourceRect;

        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank; 
        private Song music;

        public bool IsFinished { get; set; }

        public Score(Player[] players)
        {
            this.players = players;
            pointsAreDone = false;
            IsFinished = false;
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>("Score/ScoringBackgound");
            backgroundSourceRect = new Rectangle(0, 0, background.Bounds.Width, background.Bounds.Height);

            podium = Content.Load<Texture2D>("Score/Podium");
            podiumSourceRect = new Rectangle(0, 0, podium.Bounds.Width, podium.Bounds.Height);

            points = new List<Vector2>();
            pointBar = Content.Load<Texture2D>("Score/ScoreBar");
            pointSourceRect = new Rectangle(0, 0, pointBar.Bounds.Width, pointBar.Bounds.Height);
            PopulatePointList();

            font = Content.Load<SpriteFont>("Score/ScoreSpriteFont");
            titleFont = Content.Load<SpriteFont>("Score/ScoreTitleSpriteFont");

            miniController = Content.Load<Texture2D>("Instruction/LittleController");
            miniControlSourceRect = new Rectangle(0, 0, miniController.Bounds.Width, miniController.Bounds.Height);
            check = Content.Load<Texture2D>("Instruction/check");
            checkSourceRect = new Rectangle(0, 0, check.Bounds.Width, check.Bounds.Height);

            //audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");
           // waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
           // soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");
            //music = Content.Load<Song>("Instruction/instructionMusic");
            //MediaPlayer.Play(music);
            //MediaPlayer.IsRepeating = true;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (numberOfPointsShowing < points.Count)
            {
                pointCounter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (pointCounter >= timeInterval)
                {
                    numberOfPointsShowing++;
                    pointCounter = 0;
                }
            }
            else if (!pointsAreDone)
            {
                pointsAreDone = true;
            }

            if (pointsAreDone)
            {
                if (keyboardState.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
                {
                    isPlayer1Ready = true;
                    isPlayer2Ready = true;
                    isPlayer3Ready = true;
                    isPlayer4Ready = true;
                   // soundBank.PlayCue("Jump");
                }
                else
                {
                    isPlayer1Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.One), isPlayer1Ready);
                    isPlayer2Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Two), isPlayer2Ready);
                    isPlayer3Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Three), isPlayer3Ready);
                    isPlayer4Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Four), isPlayer4Ready);
                }
            }

            if (isPlayer1Ready && isPlayer2Ready && isPlayer3Ready && isPlayer4Ready)
            {
                MediaPlayer.Stop();
                IsFinished = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(background, backgroundSourceRect, Color.White);

            spriteBatch.DrawString(titleFont, Strings.ScoreTitle,
                                   new Vector2(((screenWidth / 2) - (Strings.ScoreTitle.Length / 2 * arial50CharacterSpacing)),
                                   35), Color.WhiteSmoke);

            DrawPodiums(spriteBatch);
            DrawPoints(spriteBatch);

            if (pointsAreDone)
            {
                DrawScores(spriteBatch);
                spriteBatch.DrawString(font, Strings.PressAContinue, 
                                       new Vector2(((screenWidth / 2) - (Strings.PressAContinue.Length / 2 * arial20CharacterSpacing)),
                                       podiumStart + (3.5f * miniController.Bounds.Height)), Color.WhiteSmoke);
                DrawControllers(spriteBatch);
                DrawCheck(spriteBatch);
            }

        }

        private void DrawCheck(SpriteBatch spriteBatch)
        {
            Vector2 position;
            float increment = screenWidth / (players.Length * 2);
            if (isPlayer1Ready && players.Length >= 1)
            {
                position = new Vector2(increment - check.Bounds.Width / 2,
                                       podiumStart + (miniController.Bounds.Height));
                spriteBatch.Draw(check, position, checkSourceRect, Color.White);
            }

            if (isPlayer2Ready && players.Length >= 2)
            {
                position = new Vector2(increment * 2 + increment - check.Bounds.Width / 2,
                                      podiumStart + (miniController.Bounds.Height));
                spriteBatch.Draw(check, position, checkSourceRect, Color.White);
            }

            if (isPlayer3Ready && players.Length >= 3)
            {
                position = new Vector2(increment * 4 + increment - check.Bounds.Width / 2,
                                       podiumStart + (miniController.Bounds.Height));
                spriteBatch.Draw(check, position, checkSourceRect, Color.White);
            }

            if (isPlayer4Ready && players.Length >= 4)
            {
                position = new Vector2(increment * 6 + increment - check.Bounds.Width / 2,
                                      podiumStart + (miniController.Bounds.Height));
                spriteBatch.Draw(check, position, checkSourceRect, Color.White);
            }
        }

        private void DrawControllers(SpriteBatch spriteBatch)
        {
            Vector2 position;
            float increment = screenWidth / (players.Length * 2);
            Color[] controllerColors = new Color[4] { Color.CornflowerBlue, Color.Pink, Color.DarkGray, Color.Lime };
            for (int i = 0; i <= players.Length - 1; i++)
            {
                position = new Vector2(increment * 2 * i + increment - miniController.Bounds.Width / 2, 
                                       podiumStart + (1.5f*miniController.Bounds.Height));
                spriteBatch.Draw(miniController, position, miniControlSourceRect, controllerColors[i]);
            }
        }
        
        private void DrawPodiums(SpriteBatch spriteBatch)
        {
            Vector2 position;
            float increment = screenWidth / (players.Length * 2);
            for (int i = 0; i <= players.Length-1; i++)
            {
                position = new Vector2(increment * 2 * i + increment - podium.Bounds.Width / 2, podiumStart);
                spriteBatch.Draw(podium, position, podiumSourceRect, Color.White);
            }
        }

        private void DrawPoints(SpriteBatch spriteBatch)
        {
            int count = 0;
            Vector2 position;
            float increment = screenWidth / (players.Length * 2);
            foreach(Vector2 point in points)
            {
                if (count >= numberOfPointsShowing)
                {
                    break;
                }    
                position = new Vector2(increment * 2 * point.X + increment - pointBar.Bounds.Width / 2,
                                           podiumStart - (pointBar.Bounds.Height * point.Y));
                spriteBatch.Draw(pointBar, position, pointSourceRect, players[(int)point.X].PlayerColor);
                count++;
            }
        }

        private void DrawScores(SpriteBatch spriteBatch)
        {
            Vector2 position;
            float increment = screenWidth / (players.Length * 2);
            for (int i = 0; i <= players.Length - 1; i++)
            {
                position = new Vector2((increment * 2 * i) + increment - ((players[i].Score.ToString().Length*arial50CharacterSpacing) / 2),
                                       podiumStart - (pointBar.Bounds.Height * (players[i].Score + 1.5f)));
                spriteBatch.DrawString(titleFont, players[i].Score.ToString(), position, Color.WhiteSmoke);
            }
        }

        private void PopulatePointList()
        {
            for (int i = 0; i <= players.Length - 1; i++)
            {
                for (int j = 1; j <= players[i].Score; j++)
                {
                    points.Add(new Vector2(i, j));
                }
            }
        }

        private bool UpdatePlayerStatus(GamePadState state, bool player)
        {
            GamePadState gamePadState = state;
            if (gamePadState.IsButtonDown(Buttons.A))
            {
                if (!player)
                {
                    //soundBank.PlayCue("Jump");
                    player = true;
                }
            }
            else if (gamePadState.IsButtonDown(Buttons.B))
            {
                if (player)
                {
                   // soundBank.PlayCue("Slide");
                    player = false;
                }
            }
            return player;
        }

    }
}
