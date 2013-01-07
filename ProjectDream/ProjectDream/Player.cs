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
    public class Player
    {
        public Texture2D sprite;
        public Vector2 position{ get; protected set; }
        protected Vector2 origin;

        public bool IsInitialized { get; private set; }
        public int Score { get; set; }
        public Color PlayerColor { get; set; }
        
        public Player()
        {
            IsInitialized = false;
        }

        public virtual void Initialize(Texture2D sprite, Vector2 position, Vector2 origin)
        {
            this.sprite = sprite;
            this.position = position;
            this.origin = origin;
            IsInitialized = true;
        }

        public virtual void KeyboardActions(){ }

        public virtual void Move(float elapsedTime) { }

        public virtual void Draw(SpriteBatch spriteBatch){ }
    }
}
