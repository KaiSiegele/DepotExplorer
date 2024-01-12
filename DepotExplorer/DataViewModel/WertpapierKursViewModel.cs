using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class WertpapierKursViewModel : EditObjectViewModel
    {
        public WertpapierKursViewModel()
        {
        }

        public WertpapierKursViewModel(BaseObject kurs)
          : base(kurs, true)
        {
        }

        [BaseDataProperty]
        public int Wertpapier
        {
            get
            {
                return _wertpapierid;
            }
            set
            {
                _wertpapierid = value;
                UpdateDataProperty("Wertpapier", value);
            }
        }
        private int _wertpapierid;

        [BaseDataProperty]
        public int Jahr
        {
            get
            {
                return _jahrid;
            }
            set
            {
                _jahrid = value;
                UpdateDataProperty("Jahr", value);
            }
        }
        private int _jahrid;

        [BaseDataProperty]
        public double Wert
        {
            get
            {
                return _wert;
            }
            set
            {
                _wert = value;
                UpdateDataProperty("Wert", value);
            }
        }
        private double _wert;
    }

    internal class WertpapierKursViewModels : ObjectViewModels
    {
        public WertpapierKursViewModels(BaseObjects bos, string sortCriterium)
            : base(bos, sortCriterium)
        {
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new WertpapierKursViewModel(baseObject);
        }
        private bool UseAll { get; set; }
    }

    class FondKursViewModels : WertpapierKursViewModels
    {
        public FondKursViewModels(BaseObjects bos, string sortCriterium)
            : base(bos, sortCriterium)
        {
            Anbieter = BaseObject.NotSpecified;
        }

        public int Anbieter { get; set; }

        public void UpdateAnbieter(int anbieter)
        {
            Anbieter = anbieter;
            RefreshViewModels();
        }
        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            bool result = false;
            Kurs kurs = baseObject as Kurs;
            if (kurs != null)
            {
                result = kurs.Wertpapier == BaseObject.NotSpecified || CachedData.Fonds.IsFondFromAnbieter(kurs.Wertpapier, Anbieter);
            }
            else
            {
                result = false;
            }
            return result;
        }
    }

    class AktieKursViewModels : WertpapierKursViewModels
    {
        public AktieKursViewModels(BaseObjects bos, string sortCriterium)
            : base(bos, sortCriterium)
        {
            Land = BaseObject.NotSpecified;
        }

        public int Land { get; set; }

        public void UpdateLand(int land)
        {
            Land = land;
            RefreshViewModels();
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            bool result = false;
            Kurs kurs = baseObject as Kurs;
            if (kurs != null)
            {
                result = kurs.Wertpapier == BaseObject.NotSpecified || CachedData.Aktien.IsAktieFromLand(kurs.Wertpapier, Land);
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}