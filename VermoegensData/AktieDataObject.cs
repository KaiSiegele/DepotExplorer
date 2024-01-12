using Basics;
using Persistence;

namespace VermoegensData
{
    public class AktieDataObject : WertpapierDataObject
    {
        public AktieDataObject(BaseObject aktie, Kurse kurse = null)
            : base(aktie)
        {
            _kurse = kurse;
        }

        protected override bool ExecuteCommand(CommandTyp commandTyp)
        {
            BaseObject.CheckParameter<Aktie>(commandTyp.ToString() + " Aktie", MainObject);
            return CommandInterpreter.ExecuteCommand(commandTyp, new WertpapierCommand(MainObject, _aktieQuery, CachedData.Aktien, _wertpapierQuery, _kursQuery, _kurse));
        }

        private readonly Kurse _kurse = null;
        private readonly WertpapierQuery _wertpapierQuery = new WertpapierQuery();
        private readonly AktieQuery _aktieQuery = new AktieQuery();
        private readonly KursQuery _kursQuery = new KursQuery();
    }
}
