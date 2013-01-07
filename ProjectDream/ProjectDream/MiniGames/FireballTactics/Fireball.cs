using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.FireballTactics
{
    class Fireball
    {
        private Rectangle fireballRectangle = new Rectangle(0, 0, 64, 64);
        public Texture2D sprite { get; private set; }
        public Vector2 position { get; private set; }
        public Vector2 velocity;

        private float scale { get; set; }
        private Vector2 size;
        private Vector2 origin { get; set; }
        private float angle;
        public int bounces;

        private bool onGround { get; set; }
        public static float groundLevel;
        public bool active;

        public Fireball(Texture2D newSprite, Vector2 newPosition, Vector2 newVelocity, float newScale)
        {
            sprite = newSprite;
            position = newPosition;
            velocity = newVelocity;
            scale = newScale;
            size = new Vector2(fireballRectangle.Width * scale, fireballRectangle.Height * scale);
            origin = new Vector2(fireballRectangle.Width / 2, fireballRectangle.Height / 2);
            angle = 0.0f;
            bounces = 0;
            onGround = false;
            active = true;
        }

        public void Move()
        {
            if (scale == 0.5f)
            {
                BounceEffect();
            }
            position += velocity;
        }

        public bool collides(FT_Character player)
        {
            Rectangle fireballRectangle = new Rectangle((int)position.X, (int)(position.Y), (int)size.X, (int)size.Y);
            Rectangle playerRectangle = new Rectangle((int)player.position.X, (int)(player.position.Y + (64 - size.Y)), (int)player.collisionSize.X, (int)player.collisionSize.Y);
            if (fireballRectangle.Intersects(playerRectangle) && active == true)
            {
                active = false;
                return true;
            }
            return false;
        }

        private void BounceEffect()
        {
            if (position.Y > groundLevel)
            {
                velocity.Y = -20.0f;
                bounces++;
            }
            else
            {
                velocity.Y += 0.75f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            angle += 30.0f;
            if (angle > 360)
            {
                angle = angle % 360.0f;
            }
            float rotation = MathHelper.ToRadians(angle);

            spriteBatch.Draw(sprite, position, fireballRectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }


    }
}
