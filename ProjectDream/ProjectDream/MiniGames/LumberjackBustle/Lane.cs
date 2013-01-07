using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    public class Lane
    {
        public const int HEIGHT = 180;
        public const int WIDTH = 45300;
        Vector2 position;
        Texture2D background;
        Texture2D line;

        public List<Obstacle> obstacles;

        public void Initialize(Vector2 position, Texture2D background, Texture2D line)
        {
            this.position = position;
            this.background = background;
            this.line = line;

            obstacles = new List<Obstacle>();
        }

        public void AddObstacle(Obstacle obs)
        {
            obstacles.Add(obs);
        }

        public void Update(GameTime gameTime)
        {
            obstacles.ForEach(obs =>
                {
                    if (obs.IsAlive)
                        obs.Update(gameTime);
                });

            obstacles.ForEach(obs =>
                {
                    if (obs.IsReadyForDelete)
                        obstacles.Remove(obs);
                });
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            for (int i = 0; i < camera.Position.X + 1500; i += background.Width)
            {
                spriteBatch.Draw(background, new Vector2(position.X + i - camera.Position.X, position.Y), Color.White);
                if (i > WIDTH)
                {
                    spriteBatch.Draw(line, new Vector2(position.X + i - camera.Position.X, position.Y + 155), new Rectangle(0, 0, background.Width, 15), Color.White);
                
                }
                if (i < 700)
                {
                    spriteBatch.Draw(line, new Vector2(position.X + i - camera.Position.X, position.Y + 155), new Rectangle(0,0, background.Width, 15) , Color.White);
                }
            }
            foreach (Obstacle o in obstacles)
            {
                if (o != null) 
                {               
                    if (o.worldPosition.X > camera.Position.X + 1500)
                    {
                        return;
                    }
                    camera.DrawPosition(o, spriteBatch);
                }
            }
        }

    }
}
