using System.Collections.Generic;
using Basics;
using Tools;

namespace VermoegensData
{
    public enum FondTyp
    {
        None,
        Geldmarkt,
        Renten,
        Aktien,
        Gemischt,
    }

    public class Fond : Wertpapier
    {
        internal static void AddValidations()
        {
            BaseValidations.AddValidation<Fond>(new WertpapierNameValidation());
            BaseValidations.AddValidation<Fond>(new EnumValidation<FondTyp>("Typ"));
            BaseValidations.AddValidation<Fond>(new WKNValidation());
            BaseValidations.AddValidation<Fond>(new ISINValidation());
            BaseValidations.AddValidation<Fond>(new LinkValidation("Anbieter"));
        }

        public static Fond CreateFond(int anbieter)
        {
            Fond fond = BaseObject.CreateDefault<Fond>(ObjectState.Inserted);
            fond.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Wertpapier);
            fond.Anbieter = anbieter;
            return fond;
        }

        public Fond()
          : base(WertpapierArt.Fond)
        {
            ISIN = string.Empty;
            WKN = string.Empty;
            Anbieter = NotSpecified;
            Typ = FondTyp.None;
            Theme = NotSpecified;
            Region = NotSpecified;
        }

        public override string ToString()
        {
            return Properties.Resources.Fond + " " + Name;
        }

        public int Anbieter { get; set; }
        public FondTyp Typ { get; set; }
        public int Region { get; set; }
        public int Theme { get; set; }
    }

    public class Fonds : BaseObjects
    {
        public override string ToString()
        {
            return Misc.GetNumberDescription(Count, Properties.Resources.Fond, Properties.Resources.Fonds);
        }

        public IEnumerable<Fond> GetFondsFromAnbieter(int anbieterId)
        {
            return GetObjects<Fond>(f => f.Anbieter == anbieterId);
        }

        public bool IsFondFromAnbieter(int id, int anbieterId)
        {
            bool result = false;
            Fond fond = GetObject<Fond>(id);
            if (fond != null)
            {
                result = fond.Anbieter == anbieterId;
            }
            return result;
        }
    }
}
