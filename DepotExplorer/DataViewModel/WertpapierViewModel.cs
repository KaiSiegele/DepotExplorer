using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class WertpapierViewModel : ViewObjectViewModel
    {
        public WertpapierViewModel(BaseObject bo)
          : base(bo)
        {
        }

        public override string ToString()
        {
            return Name + " ," + WKN;
        }

        public string Name
        {
            get
            {
                return _bo != null ? _bo.Name : string.Empty;
            }
        }

        public WertpapierArt WertpapierArt
        {
            get
            {
                var wp = GetWertpapier();
                return wp != null ? wp.Art : WertpapierArt.None;
            }
        }

        public string WKN
        {
            get
            {
                var wp = GetWertpapier();
                return wp != null ? wp.WKN : string.Empty;
            }
        }

        public string ISIN
        {
            get
            {
                var wp = GetWertpapier();
                return wp != null ? wp.WKN : string.Empty;
            }
        }

        private Wertpapier GetWertpapier()
        {
            return _bo as Wertpapier;
        }
    }
}
