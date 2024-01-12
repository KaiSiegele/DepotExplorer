using Basics;
using UserInterface;

namespace DepotExplorer
{
    class BankViewModel : EditObjectViewModel
    {
        public BankViewModel()
        {
        }

        public BankViewModel(BaseObject bo)
          : base(bo)
        {
        }


        [BaseDataProperty]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                UpdateDataProperty("Name", value);
            }
        }
        private string _name;

        [BaseDataProperty]
        public string Beschreibung
        {
            get
            {
                return _beschreibung;
            }
            set
            {
                _beschreibung = value;
                UpdateDataProperty("Beschreibung", value);
            }
        }
        private string _beschreibung;

        [BaseDataProperty]
        public string Blz
        {
            get
            {
                return _blz;
            }
            set
            {
                _blz = value;
                UpdateDataProperty("Blz", value);
            }
        }
        private string _blz;
    }

    class BankViewModels : ObjectViewModels
    {
        public BankViewModels(BaseObjects bos)
          : base(bos)
        {
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new BankViewModel(baseObject);
        }
    }
}
