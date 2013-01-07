using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDream.Tables;

namespace ProjectDream.MiniGames.BumperBalls
{
    class BB_Instruction : Instruction
    {
        public BB_Instruction()
        {
            description = Strings.DescriptionBB;
            title = Strings.TitleBB;
            directionalPad = Strings.DirectionalPadBB;
        }

        public override void Load(ContentManager Content)
        {
           background = Content.Load<Texture2D>("BumperBalls/prettyUnicorns");
            base.Load(Content);
        }
    }
}
