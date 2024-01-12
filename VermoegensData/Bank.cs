using Basics;
using Tools;

namespace VermoegensData
{
    public class Bank : BaseObject
    {
        internal static void AddValidations()
        {
            BaseValidations.AddValidation<Bank>(new UniqueNameValidation("Name", 3, 16, CachedData.Banken));
            BaseValidations.AddValidation<Bank>(new StringValidation("Beschreibung", 6, 20));
            BaseValidations.AddValidation<Bank>(new BLZValidation());
        }

        public static Bank CreateBank()
        {
            Bank bank = BaseObject.CreateDefault<Bank>(ObjectState.Inserted);
            bank.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Bank);
            return bank;
        }

        public Bank()
        {
            Beschreibung = string.Empty;
            Blz = string.Empty;
        }

        public override string ToString()
        {
            return Properties.Resources.Bank + " " + Name;
        }

        public string Beschreibung { get; set; }
        public string Blz { get; set; }
    }
    
    public class Banken : BaseObjects
    {
        public override string ToString()
        {
            return Misc.GetNumberDescription(Count, Properties.Resources.Bank, Properties.Resources.Banken);
        }

        public Bank GetBank(int id)
        {
            return GetObject<Bank>(id);
        }

        public Bank GetBankByBlz(string blz)
        {
            return GetFirstObject<Bank>(b => b.Blz == blz);
        }
    }
}
