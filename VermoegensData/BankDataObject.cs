using System.Diagnostics;
using Basics;
using Persistence;

namespace VermoegensData
{
    public class BankDataObject : BaseDataObject
    {
        public BankDataObject(BaseObject bank, Addresse addresse = null)
            : base(bank)
        {
            _addresse = addresse;
        }

        protected override bool ExecuteCommand(CommandTyp commandTyp)
        {
            BaseObject.CheckParameter<Bank>(commandTyp.ToString() + " Bank", MainObject);
            if (commandTyp == CommandTyp.Remove)
            {
                Debug.Assert(_addresse == null);
                _addresse = new Addresse();
                _addresse.OwnerId = MainObject.Id;
                _addresse.OwnerType = OwnerType.Bank;
            }
            Debug.Assert(_addresse != null);

            return CommandInterpreter.ExecuteCommand(commandTyp, new BankCommand(MainObject, _bankQuery, CachedData.Banken, _addresse, _addressQuery));
        }

        private Addresse _addresse = null;
        private readonly BankQuery _bankQuery = new BankQuery();
        private readonly AddressQuery _addressQuery = new AddressQuery();
    }
}
