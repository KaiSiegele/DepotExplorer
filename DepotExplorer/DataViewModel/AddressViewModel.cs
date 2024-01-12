using Basics;
using UserInterface;

namespace DepotExplorer
{
    class AddressViewModel : EditObjectViewModel
    {
        public AddressViewModel()
        {

        }
        public AddressViewModel(BaseObject addresse)
            : base(addresse)
        {
        }

        [BaseDataProperty]
        public string Strasse
        {
            get
            {
                return _strasse;
            }
            set
            {
                _strasse = value;
                UpdateDataProperty("Strasse", _strasse);
            }
        }
        private string _strasse;

        [BaseDataProperty]
        public string Hausnummer
        {
            get
            {
                return _hausnummer;
            }
            set
            {
                _hausnummer = value;
                UpdateDataProperty("Hausnummer", _hausnummer);
            }
        }
        private string _hausnummer;

        [BaseDataProperty]
        public string PLZ
        {
            get
            {
                return _plz;
            }
            set
            {
                _plz = value;
                UpdateDataProperty("PLZ", _plz);
            }
        }
        private string _plz;

        [BaseDataProperty]
        public string Ort
        {
            get
            {
                return _ort;
            }
            set
            {
                _ort = value;
                UpdateDataProperty("Ort", _ort);
            }
        }
        private string _ort;

        [BaseDataProperty]
        public string Land
        {
            get
            {
                return _land;
            }
            set
            {
                _land = value;
                UpdateDataProperty("Land", _land);
            }
        }
        private string _land;
    }
}
