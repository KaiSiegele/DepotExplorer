using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class AktieViewModel : EditObjectViewModel
    {
        public AktieViewModel()
        {

        }

        public AktieViewModel(BaseObject bo)
          : base(bo)
        {
        }

        public override string ToString()
        {
            return Name;
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
        public int Land
        {
            get
            {
                return _land;
            }
            set
            {
                _land = value;
                UpdateDataProperty("Land", value);
            }
        }
        private int _land;

        [BaseDataProperty]
        public string Branche
        {
            get
            {
                return _branche;
            }
            set
            {
                _branche = value;
                UpdateDataProperty("Branche", value);
            }
        }
        private string _branche;

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
    }
    class AktieViewModels : ObjectViewModels
    {
        public AktieViewModels(BaseObjects bos)
          : this(BaseObject.NotSpecified, bos)
        {
        }

        public AktieViewModels(int land, BaseObjects bos)
          : base(bos)
        {
            Land = land;
        }

        public int Land { get; set; }

        public void UpdateLand(int land)
        {
            Land = land;
            RefreshViewModels();
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            if (Land == BaseObjectConverter.All)
            {
                return true;
            }
            else
            {
                Aktie aktie = baseObject as Aktie;
                if (aktie != null)
                {
                    return aktie.Land == Land;
                }
                else
                {
                    return false;
                }
            }
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new AktieViewModel(baseObject);
        }
    }
}
