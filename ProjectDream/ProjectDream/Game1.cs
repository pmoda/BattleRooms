
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        GameState gameState;
        MiniGame currentMiniGame;
        String[] miniGames;
        int miniGameIndex;

        MiniGameFactory miniGameFactory;
        PlayerFactory playerFactory;
        Story story;

        Score scoreBoard;
        Menus menus;
        bool storyMode;
        private GamePadState currentPadState;
        private GamePadState previousPadState;

        Song music;

        public Game1()
        {            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            playerFactory = new PlayerFactory();

            miniGameFactory = new MiniGameFactory(playerFactory.Players);
            miniGames = new String[4];
            miniGameIndex = 0;

            Strings.Culture = CultureInfo.CurrentCulture;
            story = new Story(playerFactory.Players);
            menus = new Menus();
            storyMode = false;
            gameState = GameState.MenuScreen;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            menus.LoadContent(Content);
            story.LoadContent(Content);
            music = Content.Load<Song>("menumusic");
            MediaPlayer.Play(music);
            // load all the sprites for the game, and create objects where needed for these
            // sprites            
            //currentMiniGame.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if ( Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                graphics.ToggleFullScreen();
            }
            
            switch (gameState)
            {
            case GameState.MenuScreen:
            {
                if (story.storyDone)
                {
                    menus.State = Menus.MenuState.Credits;
                }
                story.Reset();
                menus.Update(gameTime);

                if (menus.StoryChosen)
                {
                    playerFactory.Reset();
                    //Order of mini games to play in the story mode (hard coded for demo)
                    miniGames[0] = Strings.TitleLumberjack;
                    miniGames[1] = Strings.TitleBB;
                    miniGames[2] = Strings.TitleFT;
                    miniGames[3] = Strings.TitleDuel;

                    gameState = GameState.StoryScene;
                }

                if (menus.ExitChosen)
                {
                    this.Exit();
                }

                if (menus.MiniGameChosen != null)
                {
                    playerFactory.Reset();
                    currentMiniGame = miniGameFactory.GetMiniGameByName(menus.MiniGameChosen);
                    currentMiniGame.Graphics = graphics;
                    currentMiniGame.LoadContent(Content);

                    gameState = GameState.MiniGame;
                }
                break;
            }
            case GameState.MiniGame:
            {
                currentMiniGame.Update(gameTime);
                if (currentMiniGame.GameOver || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                {
                    // show scores
                    scoreBoard = new Score(playerFactory.Players);
                    scoreBoard.LoadContent(Content);
                    gameState = GameState.Score;
                }
                break;
            }
            case GameState.Score:
            {
                story.Cut();
                scoreBoard.Update(gameTime);
                if (scoreBoard.IsFinished)
                {
                    if (menus.StoryChosen)
                    {
                        miniGameIndex++;
                        gameState = GameState.StoryScene;
                        scoreBoard = null;
                    }
                    else
                    {
                        gameState = GameState.MenuScreen;
                        MediaPlayer.Play(music);
                        menus.MiniGameChosen = null;
                        scoreBoard = null;
                    }
                }
                break;
            }
            case GameState.StoryScene:
            {
                previousPadState = currentPadState;
                currentPadState = GamePad.GetState(PlayerIndex.One);
                // if (miniGameIndex == 4) then make sure to not load the next one, cause there isn't any
                // and load the final story screen
                if (currentPadState.IsButtonDown(Buttons.A) && !previousPadState.IsButtonDown(Buttons.A))
                {
                    story.NextPage();
                }
                // else Play the story scene here (text whatever)
                if (story.storyDone)
                {
                    miniGameIndex = 0;
                    gameState = GameState.MenuScreen;
                    MediaPlayer.Play(music);
                    menus.Reset();
                    return;
                }
                if (story.ending)
                {
                    story.End();
                    if (!story.ending)
                    {
                        gameState = GameState.MenuScreen;
                        MediaPlayer.Play(music);
                        miniGameIndex = 0;
                        menus.Reset();
                        break;
                    }
                } 
                else if (story.intro)
                {
                    story.Introduction();
                }
                else if (story.cutscene)
                {
                    ////After the player either skips the scene or it ends, load the next mini game
                    //currentMiniGame = miniGameFactory.GetMiniGameByName(miniGames[miniGameIndex]);
                    //currentMiniGame.Graphics = graphics;
                    //currentMiniGame.LoadContent(Content);

                    //gameState = GameState.MiniGame;
                    break;
                }
                else
                {
                    //If the last story scene just played, go back to main menu game state
                    if (miniGameIndex == 4)
                    {
                        story.NextPage();
                        story.ending = true;
                        break;
                    }
                    //After the player either skips the scene or it ends, load the next mini game
                    currentMiniGame = miniGameFactory.GetMiniGameByName(miniGames[miniGameIndex]);
                    currentMiniGame.Graphics = graphics;
                    currentMiniGame.LoadContent(Content);

                    gameState = GameState.MiniGame;
                    break;
                }
                break;
            }
            case GameState.Victory:
            {
                // compute who won

                // make sure victory music screen is playing

                // input to advance, or whatever
                break;
            }
            default:
            {
                break;
            }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (gameState)
            {
            case GameState.MenuScreen:
            {
                // draw the title screen, and other screens for help, credits etc
                // when needed
                menus.Draw(spriteBatch);
                break;
            }
            case GameState.MiniGame:
            {
                // delegate drawing to the MiniGame
                currentMiniGame.Draw(spriteBatch);
                break;
            }
            case GameState.Score:
            {
                scoreBoard.Draw(spriteBatch);
                break;
            }
            case GameState.StoryScene:
            {
                // show the story scene for the appropriate mini game
                story.Draw(spriteBatch);
                break;
            }
            case GameState.Victory:
            {
                // screen showing points for mini games played and how many left to go
                // before the end of the game

                break;
            }
            default:
            {
                break;
            }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
