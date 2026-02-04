using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class BestandViewModel : EditObjectViewModel
    {
        public BestandViewModel()
        {
        }

        public BestandViewModel(BaseObject bestand)
          : base(bestand, true)
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
        public double Anteile
        {
            get
            {
                return _anteile;
            }
            set
            {
                _anteile = value;
                UpdateDataProperty("Anteile", value);
            }
        }
        private double _anteile;

        [BaseDataProperty]
        public double Kauf
        {
            get
            {
                return _kauf;
            }
            set
            {
                _kauf = value;
                UpdateDataProperty("Kauf", value);
                UpdateSaldo();
            }
        }
        private double _kauf = 0.0;

        [BaseDataProperty]
        public double Verkauf
        {
            get
            {
                return _verkauf;
            }
            set
            {
                _verkauf = value;
                UpdateDataProperty("Verkauf", value);
                UpdateSaldo();
            }
        }
        private double _verkauf = 0.0;

        [BaseDataProperty]
        public double Dividende
        {
            get
            {
                return _dividende;
            }
            set
            {
                _dividende = value;
                UpdateDataProperty("Dividende", value);
                UpdateSaldo();
            }
        }
        private double _dividende = 0.0;
        public double Saldo
        {
            get
            {
                return _saldo;
            }
        }
        private double _saldo;

        private void UpdateSaldo()
        {
            _saldo = _verkauf + _dividende - _kauf;
            OnPropertyChanged("Saldo");
        }
    }

    class BestandViewModels : ObjectViewModels
    {
        public BestandViewModels(BaseObjects bos)
          : base(bos)
        {
            Jahr = BaseObject.NotSpecified;
        }

        public int Jahr
        {
            get; set;
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            Bestand bestand = baseObject as Bestand;
            if (bestand != null)
            {
                return bestand.Jahr == Jahr;
            }
            else
            {
                return false;
            }
        }

        public void UpdateJahr(int jahr)
        {
            Jahr = jahr;
            RefreshViewModels();
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new BestandViewModel(baseObject);
        }
    }
}
