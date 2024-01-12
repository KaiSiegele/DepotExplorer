using Basics;

namespace VermoegensData
{
    public enum StammdatenTabelle
    {
        None = 0,
        Anbieter,
        Laender,
        Themen,
        Regionen
    }

    public class Stammdatum : BaseObject
    {
        public Stammdatum()
        {
            Beschreibung = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Tabelle);
        }

        public string Beschreibung { get; set; }

        public StammdatenTabelle Tabelle { get; set; }
    }

    public class Stammdaten : BaseObjects
    {
        public void UpdateTabelle(StammdatenTabelle tabelle)
        {
            UpdateObjects<Stammdatum>(s => s.Tabelle = tabelle);
        }
    }
}
