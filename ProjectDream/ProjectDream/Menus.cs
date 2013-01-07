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
using System.Globalization;
using ProjectDream.Tables;

namespace ProjectDream
{
    public class Menus
    {
        public enum MenuState
        {
            Main = 0,
            Story,
            ChooseGame,
            Credits,
            Exit
        }

        public MenuState State { get; set; }
        public String MiniGameChosen { get; set; }
        public bool ExitChosen = false;
        public bool StoryChosen = false;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GamePadState currentPadState;
        private GamePadState previousPadState;

        private List<KeyValuePair<String, List<Texture2D> > > miniGameNameTextureList;
        private Texture2D mainMenuTexture;
        private Texture2D chooseTexture;
        private Texture2D creditsTexture;
        private Texture2D creditsText;

        private Rectangle screenShotRect;

        private int currentChosenMiniGame = 0;
        private int currentScreenShot = 0;
        private float screenShotTimer = 0f;
        private const float screenShotSwitchTime = 5000f;
        private const int NUM_OF_SCREENSHOTS = 3;

        private SpriteFont font;
        private Texture2D cursorTexture;
        private Vector2 cursorPosition;
        private Vector2 creditPosition;
        private float cursorY;
        private int cursorIndex = 0;

        public Menus()
        {
            miniGameNameTextureList = new List<KeyValuePair<String, List<Texture2D>>>();
            screenShotRect = new Rectangle(600, 100, 640, 480);
            MiniGameChosen = null;
            creditPosition = new Vector2(500, 300);
        }

        public void LoadContent(ContentManager Content)
        {
            mainMenuTexture = Content.Load<Texture2D>(@"Menus\MainMenu");
            chooseTexture = Content.Load<Texture2D>(@"Menus\Credits");
            creditsTexture = Content.Load<Texture2D>(@"Menus\Credits");
            creditsText = Content.Load<Texture2D>(@"Menus\CreditsText");

            //fill the miniGame list with all mini games and screenshots
            MiniGameFactory factory = new MiniGameFactory(null);
            var stringList = factory.GetMiniGameStrings();

            foreach (string name in stringList)
            {
                List<Texture2D> screenshots = new List<Texture2D>();

                switch (name)
                {
                    case "Lumberjack Bustle":
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Lumber1"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Lumber2"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Lumber3"));
                        break;
                    case "Bumper Balls":
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\BB1"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\BB2"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\BB3"));
                        break;
                    case "Dueling Dunes":
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Dune1"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Dune2"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\Dune3"));
                        break;
                    case "Fireball Tactics":
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\FT1"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\FT2"));
                        screenshots.Add(Content.Load<Texture2D>(@"Menus\MiniGameScreens\FT3"));
                        break;
                }

                miniGameNameTextureList.Add(new KeyValuePair<string, List<Texture2D> >(name, screenshots));
            }

            font = Content.Load<SpriteFont>("MenuFont");
            State = MenuState.Main;

            cursorTexture = Content.Load<Texture2D>(@"Menus\cursor");
            cursorPosition = new Vector2(515f, 490f);
            cursorY = 490f;
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousPadState = currentPadState;
            currentPadState = GamePad.GetState(PlayerIndex.One);

            switch (State)
            {
                case MenuState.Main:
                    if ((currentKeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter)) ||
                        (currentPadState.IsButtonDown(Buttons.A) && !previousPadState.IsButtonDown(Buttons.A)))
                    {
                        State = (MenuState)(cursorIndex + 1);

                        if (State == MenuState.ChooseGame)
                        {
                            cursorPosition = new Vector2(50, 100);
                            cursorY = 100;
                            cursorIndex = 0; 
                            currentChosenMiniGame = 0;
                        }
                    }

                    if ((currentKeyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down)) ||
                        (currentPadState.IsButtonDown(Buttons.DPadDown) && !previousPadState.IsButtonDown(Buttons.DPadDown)) ||
                        (currentPadState.ThumbSticks.Left.Y < 0 && !(previousPadState.ThumbSticks.Left.Y < 0)))
                    {
                        cursorIndex = (cursorIndex + 1) % 4;
                    }

                    if ((currentKeyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up)) ||
                        (currentPadState.IsButtonDown(Buttons.DPadUp) && !previousPadState.IsButtonDown(Buttons.DPadUp)) ||
                        (currentPadState.ThumbSticks.Left.Y > 0 && !(previousPadState.ThumbSticks.Left.Y > 0)))
                    {
                        cursorIndex = cursorIndex - 1;
                        if (cursorIndex < 0)
                        {
                            cursorIndex = 3;
                        }
                    }

                    break;
                case MenuState.Story:
                    StoryChosen = true;
                    break;
                case MenuState.ChooseGame:
                    if ((currentKeyboardState.IsKeyDown(Keys.Back) && !previousKeyboardState.IsKeyDown(Keys.Back)) ||
                        (currentPadState.IsButtonDown(Buttons.B) && !previousPadState.IsButtonDown(Buttons.B)))
                    {
                        cursorPosition = new Vector2(515f, 490f);
                        cursorY = 490f;

                        cursorIndex = 1;
                        State = MenuState.Main;
                    }

                    if ((currentKeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter)) ||
                        (currentPadState.IsButtonDown(Buttons.A) && !previousPadState.IsButtonDown(Buttons.A)))
                    {
                        MiniGameChosen = miniGameNameTextureList[currentChosenMiniGame].Key;
                    }

                    if ((currentKeyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down)) ||
                        (currentPadState.IsButtonDown(Buttons.DPadDown) && !previousPadState.IsButtonDown(Buttons.DPadDown)) ||
                        (currentPadState.ThumbSticks.Left.Y < 0 && !(previousPadState.ThumbSticks.Left.Y < 0)))
                    {
                        currentChosenMiniGame = (currentChosenMiniGame + 1) % miniGameNameTextureList.Count;
                        cursorIndex = (cursorIndex + 1) % miniGameNameTextureList.Count;
                        screenShotTimer = 0f;
                        currentScreenShot = 0;
                    }

                    if ((currentKeyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up)) ||
                        (currentPadState.IsButtonDown(Buttons.DPadUp) && !previousPadState.IsButtonDown(Buttons.DPadUp)) ||
                        (currentPadState.ThumbSticks.Left.Y > 0 && !(previousPadState.ThumbSticks.Left.Y > 0)))
                    {
                        currentChosenMiniGame = currentChosenMiniGame - 1;
                        cursorIndex = cursorIndex - 1;

                        if (currentChosenMiniGame < 0)
                        {
                            currentChosenMiniGame = miniGameNameTextureList.Count - 1;
                            cursorIndex = miniGameNameTextureList.Count - 1;
                        }

                        screenShotTimer = 0f;
                        currentScreenShot = 0;
                    }

                    screenShotTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (screenShotTimer >= screenShotSwitchTime)
                    {
                        currentScreenShot = (currentScreenShot + 1) % NUM_OF_SCREENSHOTS;
                        screenShotTimer = 0f;
                    }

                    break;
                case MenuState.Credits:
                    creditPosition += new Vector2(0, -1.0f);
                    if (creditPosition.Y >= 2000)
                    {
                        creditPosition = new Vector2(500, 300);
                    }
                    if ((currentKeyboardState.IsKeyDown(Keys.Back) && !previousKeyboardState.IsKeyDown(Keys.Back)) ||
                        (currentPadState.IsButtonDown(Buttons.B) && !previousPadState.IsButtonDown(Buttons.B)))
                    {
                        cursorPosition = new Vector2(515f, 490f);
                        cursorY = 490f;
                        creditPosition = new Vector2(500, 300);
                        cursorIndex = 2;
                        State = MenuState.Main;
                    }
                    break;

                case MenuState.Exit:
                    ExitChosen = true;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float menuFontHeight = 37;
            switch (State)
            {
                case MenuState.Main:
                    spriteBatch.Draw(mainMenuTexture, Vector2.Zero, Color.White);
                    spriteBatch.Draw(cursorTexture, cursorPosition + new Vector2(0, menuFontHeight * cursorIndex), Color.White);
                    break;
                case MenuState.Story:
                    break;
                case MenuState.ChooseGame:
                    menuFontHeight = font.MeasureString("a").Y;
                    spriteBatch.Draw(chooseTexture, Vector2.Zero, Color.White);
                    spriteBatch.Draw(miniGameNameTextureList[currentChosenMiniGame].Value[currentScreenShot], screenShotRect, Color.White);
                    spriteBatch.Draw(cursorTexture, cursorPosition + new Vector2(0, menuFontHeight * cursorIndex), Color.White);

                    int i = 0;
                    foreach (var pair in miniGameNameTextureList)
                    {
                        spriteBatch.DrawString(font, pair.Key, cursorPosition + new Vector2(cursorTexture.Width + 2, menuFontHeight * i - 10), Color.White);
                        ++i;
                    }

                    break;
                case MenuState.Credits:
                    spriteBatch.Draw(creditsTexture, Vector2.Zero, Color.White);
                    //spriteBatch.DrawString(font, ", cursorPosition + new Vector2(cursorTexture.Width + 2, menuFontHeight * i - 10), Color.White);
                    spriteBatch.Draw(creditsText, creditPosition, Color.White);
                    
                    break;
            }   
        }

        public void Reset()
        {
            State = MenuState.Main;

            creditPosition = new Vector2(500, 300);
            MiniGameChosen = null;
            ExitChosen = false;
            StoryChosen = false;
        }
    }
}
