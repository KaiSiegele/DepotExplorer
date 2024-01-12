using Basics;

namespace VermoegensData
{
    public enum WertpapierArt
    {
        None,
        Fond,
        Aktie
    }

    public class Wertpapier : BaseObject
    {
        public Wertpapier()
        {
            Art = WertpapierArt.None;
        }

        public Wertpapier(WertpapierArt art)
        {
            Art = art;
        }

        public WertpapierArt Art { get; set; }
        public string WKN { get; set; }
        public string ISIN { get; set; }
    }
}
