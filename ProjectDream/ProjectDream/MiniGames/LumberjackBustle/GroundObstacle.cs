﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    class GroundObstacle : Obstacle
    {
        public Rectangle SourceRect { get; private set; }

        public GroundObstacle()
        {
        }
        
        public void Initialize(Texture2D texture, Vector2 position)
        {
            worldPosition = new Vector2(position.X, position.Y + 170 - texture.Height);

            SourceRect = new Rectangle(0, 0, 100, 20);

            Rectangle collisionRect = new Rectangle((int)worldPosition.X + 5,
                                                    (int)worldPosition.Y,
                                                    SourceRect.Width * 2 - 10,
                                                    texture.Height);

            base.Initialize(worldPosition, texture, collisionRect, ObstacleType.Ground);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, SourceRect, Color.White);
            spriteBatch.Draw(texture, position + new Vector2(SourceRect.Width, 0), SourceRect, Color.White);
        }
    }
}
