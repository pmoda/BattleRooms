using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectDream
{
    class Story
    {
        private Player[] players;

        private int pageNumber;
        private int secondaryPageNumber;
        public int miniGameNumber;
        public bool miniGameReady;
        public bool intro;
        public bool ending;
        public bool cutscene;
        public bool storyDone; 

        int looser;
        int finalist1;
        int finalist2;
        int winner;

        private const int arial20CharacterSpacing = 13;
        private const int arial50CharacterSpacing = 33;
        private const int screenWidth = 1280;

        private Rectangle backgroundSourceRect;
        private Texture2D background;

        private SpriteFont font;        

        private String[,] script;

        public Story(Player[] playersList)
        {
            looser = 0;
            finalist1 = 0;
            finalist2 = 0;
            winner = 0;

            storyDone = false;
            intro = true;
            ending = false;
            cutscene = false;
            players = playersList;

            script = new String[7,7];
            pageNumber = 0; 
            secondaryPageNumber = 0;
            miniGameNumber = 0;
            miniGameReady = false;
            script[0,0] = "An evil wizard named Kostaba once lived in a \n\n faraway place. Despite being evil, he was a \n\n regular person when it came to hobbies. He enjoyed \n\n fishing, baseball but most of all gaming. \n\n His favorite game was Battle Rooms, a multiplayer \n\n party game (yes like Mario Party) created by \n\n four young developers. One day, Kostaba grew \n\n weary of the same old games.";
            script[1, 0] = "So Kostaba did what any rational evil wizard \n\n would do; he kidnapped the game developers \n\n and trapped them in a room. After the  wizard \n\n demanded them to create for him a new Battle Rooms \n\n game, the lead developer explained that this \n\n could not be done since the copyrights belonged \n\n to Apple.";
            script[2, 0] = "A new idea arose, the wizard could re create \n\n the game using his spectacular magic \n\n powers while the developers would compete for \n\n the championship. The prize for victory would be a \n\n single wish granted by the wizard . . .";
            script[3,0] = " . . .";
            
            script[3, 0] = "Your next test will be a team game. We will \n\n balance out the teams by matching 1st \n\n place with last place.";
            script[3, 1] = " seem\'s to be having a hard time. \n\n However! This can still be anybody\'s game. \n\n Our lucky loser has a chance to redeem themselves \n\n  with a win on this 1 vs. 3 type game.";
            script[3, 2] = " Ladies and gentlemen, our two finalists \n\n await their final test. Who will be the \n\n last one standing . . . ";
            script[3, 3] = "CONGRATULATIONS!!  \n\n you have won, what is your wish?";

            script[4, 0] = "Even though these games were fun, the hero  \n\n wished to return home with the rest of the team. \n\n They all missed their families and  their day \n\n jobs as game developers. After crying for 6 \n\n hours, the wizard finally fulfilled his promise \n\n and granted the wish. He would have to wait \n\n for Battle Rooms 2 like everyone else.";

        }

        public void Reset()
        {           
            looser = 0;
            finalist1 = 0;
            finalist2 = 0;
            winner = 0;

            intro = true;
            ending = false;
            cutscene = false;
            storyDone = false;

            secondaryPageNumber = 0;
            pageNumber = 0;
            miniGameNumber = 0;
            miniGameReady = false;
            foreach (Player p in players)
            {
                p.Score = 0;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>("Score/ScoringBackgound");
            backgroundSourceRect = new Rectangle(0, 0, background.Bounds.Width, background.Bounds.Height);
            font = Content.Load<SpriteFont>("StoryFont");
            
            players[0].sprite = Content.Load<Texture2D>(@"Character\BlueKnightSpriteSheet");
            players[1].sprite = Content.Load<Texture2D>(@"Character\PinkKnightSpriteSheet");
            players[2].sprite = Content.Load<Texture2D>(@"Character\BlackKnightSpriteSheet");
            players[3].sprite = Content.Load<Texture2D>(@"Character\GreenKnightSpriteSheet");
        }

        public void Introduction()
        {
            if (pageNumber > 2)
            {
                intro = false;
                pageNumber = 2;
            }
            else
            {
               // pageNumber++;
            }
        }

        public void Cut()
        {            
            cutscene = true;
            pageNumber = 3;
        }
        public void End()
        {
            secondaryPageNumber = 0;
            if (pageNumber > 4)
            {
                ending = false;
                storyDone = true;
            }
            else
            {
                // pageNumber++;
            }
        }
        public void NextPage()
        {
            if (pageNumber == 3 && !ending)
            {
                cutscene = false;
                secondaryPageNumber++;
                if (secondaryPageNumber > 3)
                {
                    secondaryPageNumber = 0;
                    pageNumber++;
                    ending = true;
                    //cutscene = true;
                }
                return;
            }
            pageNumber++;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            looser=0;
            finalist1=0;
            finalist2=0;
            winner = 0;
            
            for (int i = 1; i < 4; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Score > players[finalist1].Score)
                    {
                        finalist2 = finalist1;
                        finalist1 = i;
                    }
                    else if (i == 1)
                    {
                        finalist2 = i;
                    }
                    else if (players[i].Score > players[finalist2].Score)
                    {
                        finalist2 = i;
                    }
                }
            }
            winner = finalist1;
            // whoever's the loser will be on top.            
            for (int i = 1; i < 4; i++)
            {
                if (players[i] != null)
                {
                    if (players[i].Score < players[looser].Score)
                    {
                        looser = i;
                    }
                }
            }
            Vector2 playerPosition = new Vector2 (screenWidth/2, 500 );
            Rectangle playerSource = new Rectangle(0, 0, 64, 64);

            spriteBatch.Draw(background, backgroundSourceRect, Color.White);

            if (secondaryPageNumber == 1)
            {
                spriteBatch.DrawString(font, "Player " + (looser + 1).ToString() + script[pageNumber, secondaryPageNumber], new Vector2(100, 100), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(players[looser].sprite, playerPosition, playerSource, Color.White);
            }
            else
            {
                if (secondaryPageNumber == 2)
                {
                    spriteBatch.Draw(players[finalist1].sprite, playerPosition + new Vector2(-50, 0), playerSource, Color.White);
                    spriteBatch.Draw(players[finalist2].sprite, playerPosition + new Vector2(50, 0), playerSource, Color.White);
                }
                if (secondaryPageNumber == 3)
                {
                    spriteBatch.Draw(players[winner].sprite, playerPosition + new Vector2(0, 0), playerSource, Color.White);
                }
                spriteBatch.DrawString(font, script[pageNumber, secondaryPageNumber], new Vector2(100, 100), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);            
            }
            
            spriteBatch.DrawString(font, "Press 'A' to continue",
                          new Vector2(screenWidth  - 500, 660), Color.WhiteSmoke);          
        }


    }
}
