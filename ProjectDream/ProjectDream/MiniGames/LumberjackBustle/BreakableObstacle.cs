
﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    class BreakableObstacle : Obstacle
    {
        private const float xScale = 8.0f;
        private const float yScale = 5.0f;

        private const int spriteWidth = 45;
        private const int spriteHeight = 51;

        private const int logWidth = 16;
        private const int logHeight = 32;

        public Rectangle SourceRect { get; private set; }
        public bool IsExploding;

        private int xFramePosition = 0;
        private float frameTimer = 0f;

        public BreakableObstacle()
        {
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            SourceRect = new Rectangle(xFramePosition,
                                       0,
                                       spriteWidth,
                                       spriteHeight);

           // position.Y -= (spriteHeight * yScale);

            worldPosition = new Vector2(position.X, position.Y + 170 - yScale * (spriteHeight));

            // since the actual log isn't located at the top left of the texture
            // its collision is different
            Rectangle collisionRectangle = new Rectangle((int)position.X,
                                                         (int)position.Y,
                                                         (int)(logWidth * xScale),
                                                         (int)(logHeight * yScale));

            IsExploding = false;
            base.Initialize(worldPosition, texture, collisionRectangle, ObstacleType.Breakable);
        }

        public override void Update(GameTime gameTime)
        {
            if (xFramePosition == 6)
            {
                IsExploding = false;
                IsAlive = false;
                IsReadyForDelete = true;
            }

            if (IsExploding)
            {
                frameTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

                if (frameTimer >= 33)
                {
                    xFramePosition += 1;
                    frameTimer = 0;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SourceRect = new Rectangle((spriteWidth * xFramePosition), 0, spriteWidth, spriteHeight);
            spriteBatch.Draw(texture, position, SourceRect, Color.White, 0f, Vector2.Zero, new Vector2(xScale, yScale), SpriteEffects.None, 0);
        }
    }
}
