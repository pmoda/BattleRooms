using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    public class Camera
    {
        private SpriteBatch spriteRenderer;
        private Vector2 cameraPosition; // top left corner of the camera

        public Vector2 Position
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        public Camera()
        {
            cameraPosition = new Vector2(0, 0);
        }

        public Camera(SpriteBatch renderer)
        {
            spriteRenderer = renderer;
            cameraPosition = new Vector2(0, 0);
        }

        public void DrawPosition(Obstacle obstacle, SpriteBatch spriteBatch)
        {   
            // get the screen position of the node in the scene relative to world coordinates
            Vector2 drawPosition = ApplyTransformations(obstacle.worldPosition);
            obstacle.setDrawPosition(drawPosition);
            //return drawPosition;
            obstacle.Draw(spriteBatch);
        }

        private Vector2 ApplyTransformations(Vector2 nodePosition)
        {
            // apply translation
            Vector2 finalPosition = nodePosition - cameraPosition;
            return finalPosition;
        }

        public void Translate(Vector2 moveVector)
        {
            cameraPosition += moveVector;
        }
    }
}
