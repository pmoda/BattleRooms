using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDream.Tables;

namespace ProjectDream.MiniGames.Duel
{
    class Duel_Instruction : Instruction
    {
        public Duel_Instruction()
        {
            title = Strings.TitleDuel;
            description = Strings.DescriptionDuel;
            aButton = Strings.AButtonDuel;
            bButton = Strings.BButtonDuel;
            xButton = Strings.XButtonDuel;
            yButton = Strings.YButtonDuel;
        }

        public override void Load(ContentManager Content)
        {
            background = Content.Load<Texture2D>("Duel/Dunes");

            base.Load(Content);
        }
    }
}
