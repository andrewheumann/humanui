using System;
using System.Drawing;
using Grasshopper.Kernel;



namespace HumanUI
{
    public class HumanUIInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Human UI";
            }
        }

        public override string Version
        {
            get
            {
                return "Beta 0.6";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1b2ec1b3-ab86-44c1-81d4-176f567b3592");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Andrew Heumann";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "andrew@heumann.com";
            }
        }

       
    }
}
