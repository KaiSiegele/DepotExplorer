using Basics;
using Persistence;
using System.Linq;

namespace VermoegensData
{
    public abstract class WertpapierDataObject : BaseDataObject
    {
        public WertpapierDataObject(BaseObject wp)
           : base(wp)
        {
        }

        public bool FindZuordnungen()
        {
            bool result;
            Zuordnungen _zuordnungen = new Zuordnungen();
            ObjectsCommand loadZurodnungen = new ObjectsCommand(Properties.Resources.Zuordnungen, new ZuordnungQuery(), MainObject, _zuordnungen);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, loadZurodnungen))
            {
                result = _zuordnungen.Any();
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static bool LoadWertpapiere(string condition, BaseObjects wertpapiere)
        {
            return CommandInterpreter.ExecuteLoadCommand(new ObjectsCommand(Properties.Resources.Wertpapiere, new WertpapierQuery(), condition, wertpapiere));
        }
    }
}
