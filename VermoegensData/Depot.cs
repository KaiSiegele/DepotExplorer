using Basics;
using System.Collections.Generic;
using Tools;

namespace VermoegensData
{
    public class Depot : BaseObject
    {
        internal static void AddValidations()
        {
            BaseValidations.AddValidation<Depot>(new UniqueNameValidation("Name", 4, 12, CachedData.Depots));
            BaseValidations.AddValidation<Depot>(new LinkValidation("Bank"));
            BaseValidations.AddValidation<Depot>(new StringValidation("KontoNummer", 6, 20));
        }

        public Depot()
        {
            Bank = NotSpecified;
            KontoNummer = string.Empty;
        }

        public static Depot CreateDepot()
        {
            Depot depot = BaseObject.CreateDefault<Depot>(ObjectState.Inserted);
            depot.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Depot);
            return depot;
        }

        public override string ToString()
        {
            return Properties.Resources.Depot + " " + Name;
        }

        public int Bank { get; set; }
        public string KontoNummer { get; set; }
    }

    public class Depots : BaseObjects
    {
        public override string ToString()
        {
            return Misc.GetNumberDescription(Count, Properties.Resources.Depot, Properties.Resources.Depots);
        }

        public IEnumerable<Depot> GetDepotsUsingBank(int bankId)
        {
            return GetObjects<Depot>(d => d.Bank == bankId);
        }
    }
}
