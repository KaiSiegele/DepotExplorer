using UserInterface;

namespace DepotExplorer
{
    class LandSelectionViewModel : SelectionViewModel
    {
        public int Land
        {
            get
            {
                return land;
            }
            set
            {
                land = value;
                UpdateProperty("Land");
            }
        }

        private int land;
    }
}