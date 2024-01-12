using UserInterface;

namespace DepotExplorer
{
    class AnbieterSelectionViewModel : SelectionViewModel
    {
        public int Anbieter
        {
            get
            {
                return anbieter;
            }
            set
            {
                anbieter = value;
                UpdateProperty("Anbieter");
            }
        }

        private int anbieter;
    }
}
