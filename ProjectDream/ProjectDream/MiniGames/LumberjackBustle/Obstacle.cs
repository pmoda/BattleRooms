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
    public abstract class Obstacle
    {
        public bool IsInitialized { get; private set; }
        public bool IsAlive;
        public bool IsReadyForDelete;

        public Texture2D texture { get; protected set; }
        protected Vector2 position;
        public Vector2 worldPosition { get; set; }
        public Rectangle CollisionRect { get; protected set; }
        public ObstacleType Type { get; private set; }

        public Obstacle()
        {
            IsReadyForDelete = false;
            IsInitialized = false;
        }

        public void Initialize(Vector2 position, Texture2D texture, Rectangle collisionRect, ObstacleType type)
        {
            this.position = position;
            this.worldPosition = position;
            this.texture = texture;
            this.CollisionRect = collisionRect;
            this.Type = type;

            IsInitialized = true;
            IsAlive = true;
        }

        public bool Collides(Vector2 position, Vector2 size, float cameraX)
        {
            Rectangle playerRectangle = new Rectangle((int)position.X + (int)cameraX, (int)(position.Y + (64 - size.Y)), (int)size.X, (int)size.Y);
            
            if(playerRectangle.Intersects(this.CollisionRect))
                return true;

            return false;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);

        public void setDrawPosition(Vector2 drawPosition)
        {
            this.position = drawPosition;
        }
    }
}
