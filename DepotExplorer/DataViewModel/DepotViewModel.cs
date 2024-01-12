using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class DepotViewModel : EditObjectViewModel
    {
        public DepotViewModel()
        {
        }

        public DepotViewModel(BaseObject depot)
          : base(depot)
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
        public int Bank
        {
            get
            {
                return _bank;
            }
            set
            {
                UpdateBankInfo(value);
                UpdateDataProperty("Bank", value);
            }
        }
        private int _bank;


        [BaseDataProperty]
        public string KontoNummer
        {
            get
            {
                return _kontoNummer;
            }
            set
            {
                _kontoNummer = value;
                UpdateDataProperty("KontoNummer", value);
            }
        }
        private string _kontoNummer;


        public string Blz
        {
            get
            {
                return _blz;
            }
        }
        private string _blz;

        public string BankName
        {
            get
            {
                return _bankName;
            }
        }
        private string _bankName;

        private void UpdateBankInfo(int bank)
        {
            _bank = bank;
            Bank obj = CachedData.Banken.GetBank(bank);
            if (obj != null)
            {
                _blz = obj.Blz;
                _bankName = obj.Name;
            }
            else
            {
                _blz = string.Empty;
                _bankName = string.Empty;
            }
            OnPropertyChanged("Blz");
            OnPropertyChanged("Bank");
            OnPropertyChanged("BankName");
        }
    }

    class DepotViewModels : ObjectViewModels
    {
        public DepotViewModels(BaseObjects bos)
          : base(bos)
        {
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new DepotViewModel(baseObject);
        }
    }
}