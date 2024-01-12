using Basics;

namespace VermoegensData
{
    public enum OwnerType
    {
        None,
        Bank,
    }
    
    public class Addresse : BaseObject
    {
        internal static void AddValidations()
        {
            BaseValidations.AddValidation<Addresse>(new StringValidation("Strasse", 4, 28));
            BaseValidations.AddValidation<Addresse>(new StringValidation("Hausnummer", 1, 4));
            BaseValidations.AddValidation<Addresse>(new PLZValidation());
            BaseValidations.AddValidation<Addresse>(new OrtValidation());
            BaseValidations.AddValidation<Addresse>(new LandValidation());
        }

        public static Addresse CreateAddresse(int ownerId, OwnerType ownerType)
        {
            Addresse addresse = BaseObject.CreateDefault<Addresse>(ObjectState.Inserted);
            addresse.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Addresse);
            addresse.OwnerId = ownerId;
            addresse.OwnerType = ownerType;
            return addresse;
        }

        public Addresse()
        {
            OwnerId = BaseObject.NotSpecified;
            OwnerType = VermoegensData.OwnerType.None;
            Strasse = string.Empty;
            Hausnummer = string.Empty;
            PLZ = string.Empty;
            Ort = string.Empty;
            Land = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("Adresse {0} {1}", PLZ, Ort);
        }

        public int OwnerId { get; set; }
        public OwnerType OwnerType { get; set; }
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string PLZ { get; set; }
        public string Ort { get; set; }
        public string Land { get; set; }
    }
}
