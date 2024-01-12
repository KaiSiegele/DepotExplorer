using System.Collections.Generic;
using Basics;
using Tools;

namespace VermoegensData
{
    public class Aktie : Wertpapier
    {
        internal static void AddValidations()
        {
            BaseValidations.AddValidation<Aktie>(new WertpapierNameValidation());
            BaseValidations.AddValidation<Aktie>(new WKNValidation());
            BaseValidations.AddValidation<Aktie>(new ISINValidation());
            BaseValidations.AddValidation<Aktie>(new LinkValidation("Land"));
            BaseValidations.AddValidation<Aktie>(new StringValidation("Branche", 4, 20));
        }

        public static Aktie CreateAktie(int land)
        {
            Aktie aktie = BaseObject.CreateDefault<Aktie>(ObjectState.Inserted);
            aktie.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Wertpapier);
            aktie.Land = land;
            return aktie;
        }

        public Aktie()
          : base(WertpapierArt.Aktie)
        {
            ISIN = string.Empty;
            WKN = string.Empty;
            Land = NotSpecified;
            Branche = string.Empty;
            Beschreibung = string.Empty;
        }

        public override string ToString()
        {
            return Properties.Resources.Aktie + " " + Name;
        }

        public int Land { get; set; }
        public string Branche { get; set; }
        public string Beschreibung { get; set; }
    }

    public class Aktien : BaseObjects
    {
        public override string ToString()
        {
            return Misc.GetNumberDescription(Count, Properties.Resources.Aktie, Properties.Resources.Aktien);
        }

        public IEnumerable<Aktie> GetAktienFromLand(int landId)
        {
            return GetObjects<Aktie>(a => a.Land == landId);
        }

        public bool IsAktieFromLand(int id, int landId)
        {
            bool result = false;
            Aktie aktie = GetObject<Aktie>(id);
            if (aktie != null)
            {
                result = aktie.Land == landId;
            }
            return result;
        }
    }
}
