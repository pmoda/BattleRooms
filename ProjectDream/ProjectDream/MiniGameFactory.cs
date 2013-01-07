using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectDream
{
    public class MiniGameFactory
    {
        private Random rand;
        private List<KeyValuePair<MiniGamePlayerType, String>> miniGameList;
        private Player[] players;

        public MiniGameFactory(Player[] players)
        {
            this.players = players;
            rand = new Random();
            miniGameList = new List<KeyValuePair<MiniGamePlayerType, string>>(0);

            // populate list with mini games
            miniGameList.Add(new KeyValuePair<MiniGamePlayerType, String>(MiniGamePlayerType.ThreeVOne, "Fireball Tactics"));
            miniGameList.Add(new KeyValuePair<MiniGamePlayerType, String>(MiniGamePlayerType.TwoVTwo, "Lumberjack Bustle"));
            miniGameList.Add(new KeyValuePair<MiniGamePlayerType, String>(MiniGamePlayerType.FFA, "Bumper Balls"));
            miniGameList.Add(new KeyValuePair<MiniGamePlayerType, String>(MiniGamePlayerType.OneVOne, "Dueling Dunes"));
        }

        // It should be up to the user to initialize this minigame
        // with whatever textures etc that it needs via it's own
        // initialize method
        public MiniGame GetMiniGameByName(String name)
        {
            MiniGame game = null;

            switch (name)
            {
            case "Lumberjack Bustle":
            {
                game = new MiniGames.LumberjackBustle.LumberjackBustle(players);                
                break;
            }
            case "Bumper Balls":
            {
                game = new MiniGames.BumperBalls.BumperBalls(players);
                break;
            }
            case "Dueling Dunes":
            {
                game = new MiniGames.Duel.Duel(players);
                break;
            }
            case "Fireball Tactics":
            game = new MiniGames.FireballTactics.FireballTactics(players);
            break;
            }

            return game;
        }

        public MiniGame GetRandomTwoVTwoMiniGame()
        {
            var twoVTwoGames = miniGameList.Where(miniGame => miniGame.Key == MiniGamePlayerType.TwoVTwo);

            return GetMiniGameByName(twoVTwoGames.ElementAt(rand.Next(0, twoVTwoGames.Count())).Value);
        }

        public MiniGame GetRandomFFAMiniGame()
        {
            var twoVTwoGames = miniGameList.Where(miniGame => miniGame.Key == MiniGamePlayerType.FFA);

            return GetMiniGameByName(twoVTwoGames.ElementAt(rand.Next(0, twoVTwoGames.Count())).Value);
        }

        public MiniGame GetRandomThreeVOneMiniGame()
        {
            var twoVTwoGames = miniGameList.Where(miniGame => miniGame.Key == MiniGamePlayerType.ThreeVOne);

            return GetMiniGameByName(twoVTwoGames.ElementAt(rand.Next(0, twoVTwoGames.Count())).Value);
        }

        public MiniGame GetRandomOneVOneMiniGame()
        {
            var twoVTwoGames = miniGameList.Where(miniGame => miniGame.Key == MiniGamePlayerType.OneVOne);

            return GetMiniGameByName(twoVTwoGames.ElementAt(rand.Next(0, twoVTwoGames.Count())).Value);
        }

        public List<String> GetMiniGameStrings()
        {
            List<String> list = new List<String>();
            foreach (var pair in miniGameList)
            {
                list.Add(pair.Value);
            }

            return list;
        }
    }
}
