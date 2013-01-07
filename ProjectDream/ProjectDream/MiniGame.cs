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

namespace ProjectDream
{
    public abstract class MiniGame
    {
        public MiniGamePlayerType MiniGameType { get; private set; }
        public String MiniGameName { get; private set; }
        protected Player[] Players { get; set; }
        public List<Player> Characters{ get; set; }
        public bool IsInitialized { get; private set; }
        public bool GameOver { get; protected set; }
        public GraphicsDeviceManager Graphics { get; set; }

        public MiniGame()
        {
            IsInitialized = false;
        }

        // The child of this class should call this in their own Initialize method
        protected void Initialize(MiniGamePlayerType miniGameType, String miniGameName)
        {
            MiniGameType = miniGameType;
            MiniGameName = miniGameName;
            GameOver = false;
            IsInitialized = true;
        }

        public abstract void LoadContent(ContentManager Content);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
