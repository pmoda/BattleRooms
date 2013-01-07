using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectDream
{
    class PlayerFactory
    {
        private const int numberOfPlayers = 4;
        public Player[] Players { get; set; }

        public PlayerFactory()
        {
            Players = new Player[numberOfPlayers];
            PopulatePlayers();
        }

        public void Reset()
        {
            for (int i = 0; i <= numberOfPlayers - 1; i++)
            {
                Players[i].Score = 0;
            }
        }

        private void PopulatePlayers()
        {
            for (int i = 0; i <= (numberOfPlayers-1); i++)
            {
                Players[i] = new Player();
                Players[i].PlayerColor = AssignPlayerColor(i);
            }
        }

        private Color AssignPlayerColor(int color)
        {
            Color finalColor = Color.White;
            switch (color)
            {
                case 0:
                    finalColor = Color.Blue;
                    break;
                case 1:
                    finalColor = Color.Pink;
                    break;
                case 2:
                    finalColor = Color.DarkGray;
                    break;
                case 3:
                    finalColor = Color.Green;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Color assignment is invalid");
            }
            return finalColor;
        }
    }
}
