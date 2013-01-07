using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDream.Tables;

namespace ProjectDream.MiniGames.FireballTactics
{
    class FT_Instruction : Instruction
    {
        public FT_Instruction()
        {
            description = Strings.DescriptionFT;
            title = Strings.TitleFT;
            aButton = Strings.AButtonFT;
            bButton = Strings.BButtonFT;
            xButton = Strings.XButtonFT;
            directionalPad = Strings.DirectionalPadFT;
        }

        public override void Load(ContentManager Content)
        {
            background = Content.Load<Texture2D>("FireballTactics/Lava");
            base.Load(Content);
        }
    }
}

