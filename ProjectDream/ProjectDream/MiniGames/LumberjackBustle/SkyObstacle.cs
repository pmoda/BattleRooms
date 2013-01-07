
﻿using Microsoft.Xna.Framework;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    class SkyObstacle : Obstacle
    {
        private const float scaleX = 3.0f;
        private const float scaleY = 3.9f;
        const float floatingHeight = 95;

        public SkyObstacle()
        {
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            //position.Y -= Lane.HEIGHT;

            worldPosition = new Vector2(position.X, position.Y);

            Rectangle collisionRect = new Rectangle((int)position.X,
                                                    (int)position.Y,
                                                    (int)(texture.Width * scaleX),
                                                    (int)(texture.Height * scaleY));

            base.Initialize(worldPosition, texture, collisionRect, ObstacleType.Sky);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.Draw(texture, position, null, Color.White, 0.0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0.0f);
        }
    }
}
