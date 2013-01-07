using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectDream.Tables;
using System.Globalization;
using Microsoft.Xna.Framework.Audio;

namespace ProjectDream
{
    class Instruction
    {
        protected const int controllerWidth = 40;
        protected const int controllerHeight = 45;

        protected bool isPlayer1Ready;
        protected bool isPlayer2Ready;
        protected bool isPlayer3Ready;
        protected bool isPlayer4Ready;

        protected string title;
        protected string description;
        protected string aButton;
        protected string backButton;
        protected string bButton;
        protected string directionalPad;
        protected string leftBumper;
        protected string leftStick;
        protected string leftTrigger;
        protected string rightBumper;
        protected string rightStick;
        protected string rightTrigger;
        protected string startButton;
        protected string xBoxButton;
        protected string xButton;
        protected string yButton;

        protected Rectangle controllerSourceRect;
        protected Rectangle backgroundSourceRect;
        protected Song music;
        protected SpriteFont font;
        protected Texture2D background;
        protected Texture2D scrolls;
        protected Texture2D miniController;
        protected Texture2D check;
        protected Vector2 titlePosition;
        protected Vector2 buttonPosition;

        protected AudioEngine audioEngine;
        protected WaveBank waveBank;
        protected SoundBank soundBank; 
        
        public Instruction()
        {
            aButton = Strings.AButtonDefault;
            backButton = Strings.BackButtonDefault;
            bButton = Strings.BButtonDefault;
            description = Strings.DesriptionDefault;
            directionalPad = Strings.DirectionalPadDefault;
            leftBumper = Strings.LeftBumperDefault;
            leftStick = Strings.LeftStickDefault;
            leftTrigger = Strings.LeftTriggerDefault;
            rightBumper = Strings.RightBumperDefault;
            rightStick = Strings.RightStickDefault;
            rightTrigger = Strings.RightTriggerDefault;
            startButton = Strings.StartButtonDefault;
            title = Strings.TitleDefault;
            xBoxButton = Strings.XBoxButtonDefault;
            xButton = Strings.XButtonDefault;
            yButton = Strings.YButtonDefault;
        }

        public virtual void Load(ContentManager Content)
        {
            backgroundSourceRect = new Rectangle(0, 0, 1280, 720);
            controllerSourceRect = new Rectangle(125, 290, controllerWidth, controllerHeight);
            titlePosition = new Vector2(140f, 65f);
            buttonPosition = new Vector2(1060f, 270f);
            scrolls = Content.Load<Texture2D>("Instruction/InstWizardAndScrolls");
            font = Content.Load<SpriteFont>("Instruction/InstructionSpriteFont");
            miniController = Content.Load<Texture2D>("Instruction/LittleController");
            check = Content.Load<Texture2D>("Instruction/check");
           // music = Content.Load<Song>("Instruction/instructionMusic");
       ////   audioEngine = new AudioEngine(@"Content\Character\LumberjackBustle.xgs");
        //    waveBank = new WaveBank(audioEngine, @"Content\Character\Wave Bank.xwb");
        //    soundBank = new SoundBank(audioEngine, @"Content\Character\Sound Bank.xsb");
            //MediaPlayer.Play(music);
            //MediaPlayer.IsRepeating = true;
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
            {
                isPlayer1Ready = true;
                isPlayer2Ready = true;
                isPlayer3Ready = true;
                isPlayer4Ready = true;
             //   soundBank.PlayCue("Jump");
            }
            else
            {
                isPlayer1Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.One), isPlayer1Ready);
                isPlayer2Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Two), isPlayer2Ready);
                isPlayer3Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Three), isPlayer3Ready);
                isPlayer4Ready = UpdatePlayerStatus(GamePad.GetState(PlayerIndex.Four), isPlayer4Ready);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, backgroundSourceRect, Color.White);
            spriteBatch.Draw(scrolls, backgroundSourceRect, Color.White);
            DrawController(spriteBatch, controllerSourceRect, isPlayer1Ready, Color.CornflowerBlue);
            DrawController(spriteBatch, new Rectangle((controllerSourceRect.X + (controllerWidth * 2)), controllerSourceRect.Y, controllerSourceRect.Width, controllerSourceRect.Height), isPlayer2Ready, Color.Pink);
            DrawController(spriteBatch, new Rectangle((controllerSourceRect.X + (controllerWidth * 4)), controllerSourceRect.Y, controllerSourceRect.Width, controllerSourceRect.Height), isPlayer3Ready, Color.DarkGray);
            DrawController(spriteBatch, new Rectangle((controllerSourceRect.X + (controllerWidth * 6)), controllerSourceRect.Y, controllerSourceRect.Width, controllerSourceRect.Height), isPlayer4Ready, Color.Lime);
            spriteBatch.DrawString(font, title, titlePosition, Color.Black);
            spriteBatch.DrawString(font, description, new Vector2(titlePosition.X, titlePosition.Y + 35), Color.Black);
            spriteBatch.DrawString(font, leftTrigger, buttonPosition, Color.Black);
            spriteBatch.DrawString(font, rightTrigger, new Vector2(buttonPosition.X, buttonPosition.Y + 25), Color.Black);
            spriteBatch.DrawString(font, leftBumper, new Vector2(buttonPosition.X, buttonPosition.Y + 85), Color.Black);
            spriteBatch.DrawString(font, rightBumper, new Vector2(buttonPosition.X, buttonPosition.Y + 110), Color.Black);
            spriteBatch.DrawString(font, yButton, new Vector2(buttonPosition.X, buttonPosition.Y + 130), Color.Black);
            spriteBatch.DrawString(font, bButton, new Vector2(buttonPosition.X, buttonPosition.Y + 150), Color.Black);
            spriteBatch.DrawString(font, xButton, new Vector2(buttonPosition.X, buttonPosition.Y + 195), Color.Black);
            spriteBatch.DrawString(font, aButton, new Vector2(buttonPosition.X, buttonPosition.Y + 215f), Color.Black);
            spriteBatch.DrawString(font, xBoxButton, new Vector2(buttonPosition.X, buttonPosition.Y + 235f), Color.Black);
            spriteBatch.DrawString(font, startButton, new Vector2(buttonPosition.X, buttonPosition.Y + 255f), Color.Black);
            spriteBatch.DrawString(font, backButton, new Vector2(buttonPosition.X, buttonPosition.Y + 275f), Color.Black);
            spriteBatch.DrawString(font, rightStick, new Vector2(buttonPosition.X, buttonPosition.Y + 340), Color.Black);
            spriteBatch.DrawString(font, directionalPad, new Vector2(buttonPosition.X, buttonPosition.Y + 360), Color.Black);
            spriteBatch.DrawString(font, leftStick, new Vector2(buttonPosition.X, buttonPosition.Y + 380), Color.Black);
        }

        public bool ReadyToStart()
        {
            if (isPlayer1Ready && isPlayer2Ready && isPlayer3Ready && isPlayer4Ready)
            {
                MediaPlayer.Stop();
                return true;
            }
            return false;
        }

        private bool UpdatePlayerStatus(GamePadState state, bool player)
        {
            GamePadState gamePadState = state;
            if (gamePadState.IsButtonDown(Buttons.A))
            {
                if (!player)
                {
                  //  soundBank.PlayCue("Jump");
                    player = true;
                }
            }
            else if (gamePadState.IsButtonDown(Buttons.B))
            {
                if (player)
                {
                 //   soundBank.PlayCue("Slide");
                    player = false;
                }
            }
            return player;
        }

        private void DrawController(SpriteBatch spriteBatch, Rectangle rectangle, bool isReady, Color color)
        {
            spriteBatch.Draw(miniController, rectangle, color);
            if (isReady)
            {
                spriteBatch.Draw(check, rectangle, Color.White);
            }
        }
    }
}
