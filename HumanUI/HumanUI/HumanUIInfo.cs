using System;
using System.Drawing;
using Grasshopper.Kernel;



namespace HumanUI
{
    public class HumanUIInfo : GH_AssemblyInfo
    {
        public override string Name => "Human UI";

        public override string Version => Properties.Resources.CURRENT_VERSION;
        public override Bitmap Icon => Properties.Resources.Icon_24;
        public override string Description => "";

        public override Guid Id => new Guid("1b2ec1b3-ab86-44c1-81d4-176f567b3592");

        public override string AuthorName => "Andrew Heumann";
        public override string AuthorContact => "andrew@heumann.com";
    }
}
