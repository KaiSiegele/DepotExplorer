using Basics;
using Persistence;

namespace VermoegensData
{
    internal class BankCommand : ObjectCommand
    {
        public BankCommand(BaseObject bo, BaseQuery bq, BaseObjects bos, Addresse addresse, AddressQuery addressQuery) 
            : base(bo, bq, bos)
        {
            _addresse = addresse;
            _addressQuery = addressQuery;
        }

        protected override bool ExecuteLoadObject(Error error)
        {
            bool result = false;
            _addresse.OwnerId = BO.Id;
            _addresse.OwnerType = OwnerType.Bank;
            if (_addressQuery.LoadObject(_addresse, error))
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
                if (BQ.InsertObject(BO, error))
                {
                    if (_addressQuery.InsertObject(_addresse, error))
                    {
                        result = true;
                        FinishEditing(true);
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
                if (BQ.UpdateObject(BO, error))
                {
                    if (_addressQuery.UpdateObject(_addresse, error))
                    {
                        result = true;
                        FinishEditing(false);
                    }
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
                if (_addressQuery.RemoveObject(_addresse, error))
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
            _addresse.ObjectState = ObjectState.Stored;
        }

        readonly Addresse _addresse;
        readonly AddressQuery _addressQuery;
    }
}
