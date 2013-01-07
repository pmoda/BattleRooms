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
    public class ObstacleFactory
    {
        private Random rand;
        Texture2D branch;
        Texture2D root;
        Texture2D smash;

        public ObstacleFactory(ContentManager Content)
        {
            rand = new Random();
            branch = Content.Load<Texture2D>("Lumberjack Bustle/BranchObstacle");
            root = Content.Load<Texture2D>("Lumberjack Bustle/FloorObstacle");
            smash = Content.Load<Texture2D>("Lumberjack Bustle/ChopObstacle");
        }

        // give the position of the bottom of the lane
        public Obstacle GetObstacleByType(ObstacleType type, Vector2 position)
        {
            Obstacle obstacle = null;

            switch (type)
            {
                case ObstacleType.Ground:
                    obstacle = new GroundObstacle();
                    ((GroundObstacle)obstacle).Initialize(root, position);                    
                    break;
                case ObstacleType.Sky:
                    obstacle = new SkyObstacle();
                    ((SkyObstacle)obstacle).Initialize(branch, position);
                    break;
                case ObstacleType.Breakable:
                    obstacle = new BreakableObstacle();
                    ((BreakableObstacle)obstacle).Initialize(smash, position);
                    break;
            }

            return obstacle;
        }

        public Obstacle GetRandomObstacle(Vector2 position)
        {
            ObstacleType[] obstacleTypes = new ObstacleType[(int)ObstacleType.End];
            //var obstacleTypes = (ObstacleType[])Enum.GetValues(typeof(ObstacleType));
            for (int enumValue = 0; enumValue < (int)ObstacleType.End; ++enumValue)
            {
                ObstacleType next = (ObstacleType)enumValue;
                obstacleTypes[enumValue] = next;
            }
            return GetObstacleByType(obstacleTypes[rand.Next(0, obstacleTypes.Length)], position);
        }

        public ObstacleType GetRandomType()
        {
            ObstacleType[] obstacleTypes = new ObstacleType[(int)ObstacleType.End];
            //var obstacleTypes = (ObstacleType[])Enum.GetValues(typeof(ObstacleType));
            for (int enumValue = 0; enumValue < (int)ObstacleType.End; ++enumValue)
            {
                ObstacleType next = (ObstacleType)enumValue;
                obstacleTypes[enumValue] = next;
            }
            return obstacleTypes[rand.Next(0, obstacleTypes.Length)];
        }

    }
}
