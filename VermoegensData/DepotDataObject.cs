using Basics;
using Persistence;
using System;

namespace VermoegensData
{
    public class DepotDataObject : BaseDataObject
    {
        public DepotDataObject(BaseObject depot, Bestaende bestaende = null, Zuordnungen zuordnungen = null)
            : base(depot)
        {
            _bestaende = bestaende;
            _zuordnungen = zuordnungen;
        }

        protected override bool ExecuteCommand(CommandTyp commandTyp)
        {
            BaseObject.CheckParameter<Depot>(commandTyp.ToString() + " Depot", MainObject);
            return CommandInterpreter.ExecuteCommand(commandTyp, new DepotCommand(MainObject, _depotQuery, CachedData.Depots, _bestandsQuery, _bestaende, _zustandQuery, _zuordnungen));
        }

        private readonly Bestaende _bestaende;
        private readonly Zuordnungen _zuordnungen;
        private readonly BaseQuery _depotQuery = new DepotQuery();
        private readonly BaseQuery _bestandsQuery = new BestandQuery();
        private readonly BaseQuery _zustandQuery = new ZuordnungQuery();

    }
}
