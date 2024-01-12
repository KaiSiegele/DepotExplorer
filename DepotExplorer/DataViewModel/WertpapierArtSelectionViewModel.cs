using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class WertpapierArtSelectionViewModel : SelectionViewModel
    {
        public WertpapierArt WertpapierArt
        {
            get
            {
                return wertpapierArt;
            }
            set
            {
                wertpapierArt = value;
                UpdateProperty("WertpapierArt");
            }
        }

        private WertpapierArt wertpapierArt;
    }
}
