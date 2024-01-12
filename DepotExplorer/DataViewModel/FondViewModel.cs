using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class FondViewModel : EditObjectViewModel
    {
        public FondViewModel()
        {
        }

        public FondViewModel(BaseObject bo)
          : base(bo)
        {
        }

        public override string ToString()
        {
            return Name + " (" + Typ.ToString() + ")";
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
        public FondTyp Typ
        {
            get
            {
                return _typ;
            }
            set
            {
                _typ = value;
                UpdateDataProperty("Typ", value);
            }
        }
        private FondTyp _typ;

        [BaseDataProperty]
        public int Region
        {
            get
            {
                return _region;
            }
            set
            {
                _region = value;
                UpdateDataProperty("Region", value);
            }
        }
        private int _region;

        [BaseDataProperty]
        public int Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                UpdateDataProperty("Theme", value);
            }
        }
        private int _theme;

        [BaseDataProperty]
        public string WKN
        {
            get
            {
                return _wkn;
            }
            set
            {
                _wkn = value;
                UpdateDataProperty("WKN", value);
            }
        }
        private string _wkn;

        [BaseDataProperty]
        public string ISIN
        {
            get
            {
                return _ISIN;
            }
            set
            {
                _ISIN = value;
                UpdateDataProperty("ISIN", value);
            }
        }
        private string _ISIN;

        [BaseDataProperty]
        public int Anbieter
        {
            get
            {
                return _anbieter;
            }
            set
            {
                _anbieter = value;
                UpdateDataProperty("Anbieter", value);
            }
        }
        private int _anbieter;

        public string TypName
        {
            get
            {
                return _typ.ToString();
            }
        }
    }

    class FondViewModels : ObjectViewModels
    {
        public FondViewModels(BaseObjects bos)
          : this(BaseObject.NotSpecified, bos)
        {
        }

        public FondViewModels(int anbieter, BaseObjects bos)
          : base(bos)
        {
            Anbieter = anbieter;
        }
        
        public int Anbieter { get; set; }

        public void UpdateAnbieter(int anbieter)
        {
            Anbieter = anbieter;
            RefreshViewModels();
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            if (Anbieter == BaseObjectConverter.All)
            {
                return true;
            }
            else
            {
                Fond fond = baseObject as Fond;
                if (fond != null)
                {
                    return fond.Anbieter == Anbieter;
                }
                else
                {
                    return false;
                }
            }
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new FondViewModel(baseObject);
        }
    }
}
