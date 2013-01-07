using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using ProjectDream.Tables;

namespace ProjectDream.MiniGames.LumberjackBustle
{
    class LumberjackInstruction :Instruction
    {
        public LumberjackInstruction()
        {
            title = Strings.TitleLumberjack;
            description = Strings.DescriptionLumberjack;
            directionalPad = Strings.DirectionalPadLumberjack;
            aButton = Strings.AButtonLumberjack;
            xButton = Strings.XButtonLumberjack;
        }

        public override void Load(ContentManager Content)
        {
            background = Content.Load<Texture2D>("Lumberjack Bustle/InstBackForest");
            base.Load(Content);
        }
    }
}
