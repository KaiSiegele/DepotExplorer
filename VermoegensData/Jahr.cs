using System.Linq;
using Basics;
using Tools;

namespace VermoegensData
{
    public class Jahr : BaseObject
    {
        public override string ToString()
        {
            return Properties.Resources.Jahr + " " + Name;
        }
    }

    public class Jahre : BaseObjects
    {
        public override string ToString()
        {
            return Misc.GetNumberDescription(Count, Properties.Resources.Jahr, Properties.Resources.Jahre);
        }

        public int DefaultId
        {
            get
            {
                if (Keys.Any())
                {
                    return Keys.Max();
                }
                else
                {
                    return BaseObject.NotSpecified;
                }
            }
        }
    }
}
