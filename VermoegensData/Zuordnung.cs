using System.Collections.Generic;
using System.Linq;
using Basics;

namespace VermoegensData
{
    /// <summary>
    /// Zuordnungen zeigen an, welche Wertpapiere welchem Depot zugeordnet sind
    /// </summary>
    public class Zuordnung : BaseObject
    {
        public static void AddValidations()
        {
            BaseValidations.AddValidation<Zuordnung>(new LinkValidation("Wertpapier"));
            BaseValidations.AddValidation<Zuordnung>(new LinkValidation("Depot"));
        }

        internal static Zuordnung CreateZuordnung(int depotid, int wertpapierid)
        {
            Zuordnung zuordnung = BaseObject.CreateDefault<Zuordnung>(ObjectState.Inserted);
            zuordnung.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Zuordnung);
            zuordnung.Depot = depotid;
            zuordnung.Wertpapier = wertpapierid;
            return zuordnung;
        }

        public Zuordnung()
        {
            Depot = BaseObject.NotSpecified;
            Wertpapier = BaseObject.NotSpecified;
        }

        public override string ToString()
        {
            return string.Format("Zuordnung ({0}, {1}, {2})", Depot, Wertpapier, ObjectState.ToString());
        }

        public int Depot { get; set; }
        public int Wertpapier { get; set; }
    }

    public class Zuordnungen : BaseObjects
    {
        public override string ToString()
        {
            return string.Format("{0} Zuordnungen", Count);
        }

        public bool InsertZuordnung(int depotid, int wertpapierid)
        {
            Zuordnung zu = Zuordnung.CreateZuordnung(depotid, wertpapierid);
            return InsertObject(zu);
        }

        public bool RemoveZuordnung(int zuordnungId)
        {
            return SetObjectAsRemoved(zuordnungId);
        }

        public bool IsWertPapierZugeordnet(int wertpapierid)
        {
            return HasObject<Zuordnung>(z => !z.Removed && z.Wertpapier == wertpapierid);
        }

        public IEnumerable<int> WertpapierIds
        {
            get
            {
                return (from d in Values select (d as Zuordnung).Wertpapier);
            }
        }
    }
}
