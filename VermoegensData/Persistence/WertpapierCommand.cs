using Basics;
using Persistence;

namespace VermoegensData
{
    internal class WertpapierCommand : ObjectCommand
    {
        public WertpapierCommand(BaseObject bo, BaseQuery bq, BaseObjects bos, WertpapierQuery wertpapierQuery, KursQuery kursQuery, Kurse kurse)
          : base(bo, bq, bos)
        {
            _wertpapierQuery = wertpapierQuery;
            _kursQuery = kursQuery;
            _kurse = kurse;
        }

        protected override bool ExecuteLoadObject(Error error)
        {
            bool result = false;
            if (_kursQuery.LoadObjects(BO, _kurse, error))
            {
                result = base.ExecuteLoadObject(error);
            }
            return result;
        }

        protected override bool ExecuteInsertObject(Error error)
        {
            bool result = false;
            string description = GetDescription(CommandTyp.Insert);
            if (DatabaseHandle.StartTransaction(description, error))
            {
                if (_wertpapierQuery.InsertObject(BO, error))
                {
                    if (BQ.InsertObject(BO, error))
                    {
                        if (_kursQuery.ProcessAllObjects(_kurse, error))
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
                if (_kursQuery.ProcessAllObjects(_kurse, error))
                {
                    if (ObjectState.Updated == BO.ObjectState)
                    {
                        if (_wertpapierQuery.UpdateObject(BO, error))
                        {
                            result = base.ExecuteUpdateObject(error);
                        }
                    }
                    else
                    {
                        result = true;
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

        protected override bool ExecuteRemoveObject(Error error)
        {
            bool result = false;
            string description = GetDescription(CommandTyp.Remove);
            if (DatabaseHandle.StartTransaction(description, error))
            {
                if (_wertpapierQuery.RemoveObject(BO, error))
                {
                    result = base.ExecuteRemoveObject(error);
                }
                DatabaseHandle.FinishTransaction(description, result, error);
            }
            return result;
        }

        protected override void SetObjectsStored()
        {
            base.SetObjectsStored();
            _kurse.SetObjectsAsStored();
        }

        private readonly WertpapierQuery _wertpapierQuery;
        private readonly KursQuery _kursQuery;
        private readonly Kurse _kurse;
    }
}
