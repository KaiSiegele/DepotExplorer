using Basics;
using Persistence;

namespace VermoegensData
{
    public class FondDataObject : WertpapierDataObject
    {
        public FondDataObject(BaseObject fond, Kurse kurse = null)
            : base(fond)
        {
            _kurse = kurse;
        }

        protected override bool ExecuteCommand(CommandTyp commandTyp)
        {
            BaseObject.CheckParameter<Fond>(commandTyp.ToString() + " Fond", MainObject);
            return CommandInterpreter.ExecuteCommand(commandTyp, new WertpapierCommand(MainObject, _fondQuery, CachedData.Fonds, _wertpapierQuery, _kursQuery, _kurse));
        }

        private readonly Kurse _kurse = null;
        private readonly WertpapierQuery _wertpapierQuery = new WertpapierQuery();
        private readonly FondQuery _fondQuery = new FondQuery();
        private readonly KursQuery _kursQuery = new KursQuery();
    }
}
