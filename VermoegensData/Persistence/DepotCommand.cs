using Persistence;
using Basics;

namespace VermoegensData
{
    internal class DepotCommand : ObjectCommand
    {
        public DepotCommand(BaseObject bo, BaseQuery bq, BaseObjects bos, BaseQuery bestandsQuery, Bestaende bestaende, BaseQuery zuordnungQuery, Zuordnungen zuordnungen)
          : base(bo, bq, bos)
        {
            _bestandsQuery = bestandsQuery;
            _bestaende = bestaende;
            _zuordnungQuery = zuordnungQuery;
            _zuordnungen = zuordnungen;
        }


        protected override bool ExecuteLoadObject(Error error)
        {
            bool result = false;
            if (_zuordnungQuery.LoadObjects(BO, _zuordnungen, error))
            {
                if (_bestandsQuery.LoadObjects(BO, _bestaende, error))
                {
                    result = base.ExecuteLoadObject(error);
                }
            }
            return result;
        }

        protected override bool ExecuteInsertObject(Error error)
        {
            bool result = false;
            string description = GetDescription(CommandTyp.Insert);
            if (DatabaseHandle.StartTransaction(description, error))
            {
                if (BQ.InsertObject(BO, error))
                {
                    if (_zuordnungQuery.ProcessAllObjects(_zuordnungen, error))
                    {
                       if (_bestandsQuery.ProcessAllObjects(_bestaende, error))
                        {
                            result = true;
                            FinishEditing(true);
                        }
                    }
                }
                DatabaseHandle.FinishTransaction(description, result, error);
            }
            return result;
        }

        protected override bool ExecuteUpdateObject(Error error)
        {
            bool result = false;
            string description = GetDescription(CommandTyp.Update);
            if (DatabaseHandle.StartTransaction(description, error))
            {
                if (_zuordnungQuery.ProcessAllObjects(_zuordnungen, error))
                {
                    if (_bestandsQuery.ProcessAllObjects(_bestaende, error))
                    {
                        result = base.ExecuteUpdateObject(error);
                    }
                }
                if (result)
                {
                    FinishEditing(false);
                }
                DatabaseHandle.FinishTransaction(description, result, error);
            }
            return result;
        }

        protected override void SetObjectsStored()
        {
            base.SetObjectsStored();
            _bestaende.SetObjectsAsStored();
            _zuordnungen.SetObjectsAsStored();
        }

        private readonly BaseQuery _bestandsQuery;
        private readonly Bestaende _bestaende;
        private readonly BaseQuery _zuordnungQuery;
        private readonly Zuordnungen _zuordnungen;
    }
}
