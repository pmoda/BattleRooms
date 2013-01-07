using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.FireballTactics
{
    class Firewall
    {
        private Rectangle firewallBoundary = new Rectangle(0, 0, 48, 48);
        public Texture2D sprite { get; private set; }
        public Vector2 position { get; set; }

        private float scale { get; set; }
        private Vector2 size;
        private Vector2 origin { get; set; }

        public static float firewallTimer;
        private int currentFrame;
        public bool active;

        public Firewall(Texture2D newSprite, Vector2 newPosition, float newScale, int newFrame, bool isactive)
        {
            active = isactive;
            sprite = newSprite;
            position = newPosition;
            scale = newScale;
            currentFrame = newFrame;
            size = new Vector2(firewallBoundary.Width * scale, firewallBoundary.Height * scale);
            origin = new Vector2(firewallBoundary.Width / 2, firewallBoundary.Height / 2);
        }

        public bool collides(FT_Character player)
        {
            Rectangle firewallRectangle = new Rectangle((int)(position.X - size.X / 2), (int)(position.Y - size.Y / 2), (int)size.X, (int)size.Y);
            Rectangle playerRectangle = new Rectangle((int)player.position.X, (int)(player.position.Y + (64 - size.Y)), (int)player.collisionSize.X, (int)player.collisionSize.Y);
            if (firewallRectangle.Intersects(playerRectangle))
                return true;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Firewall.firewallTimer > 150.0f)
            {
                currentFrame = (currentFrame + 1) % 4;                
            }
            Rectangle firewallRectangle = new Rectangle((int)(currentFrame) * 48, 0, 48, 48);
            spriteBatch.Draw(sprite, position, firewallRectangle, Color.White, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }

    }
}
