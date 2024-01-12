using UserInterface;
using System.Data;
using Tools;

namespace DepotExplorer
{
    /// <summary>
    /// ViewModel, um alle Käufe, Verkäufe, Dividenden und
    /// den Gesamtwert zu ermittlen. Diese Daten werden aus einer
    /// Tabelle gelesen
    /// </summary>
    public abstract class SummaryViewModel : BaseViewModel
    {
        public SummaryViewModel()
        {
            _verkaeufe = 0.0;
            _kaeufe = 0.0;
            _dividende = 0.0;
            _gesamtWert = 0.0;
            _gewinn = 0.0;
        }

        /// <summary>
        /// Liest alle Werte aus der Tabelle ein
        /// </summary>
        /// <param name="dtPosten">Tabelle mit Posten</param>
        public abstract void Update(DataTable dtPosten);

        /// <summary>
        /// Setzt alle Werte zurück
        /// </summary>
        public virtual void Reset()
        {
            Verkaeufe = 0.0;
            Kaeufe = 0.0;
            Dividende = 0.0;
            GesamtWert = 0.0;
            Gewinn = 0.0;
        }

        public double Verkaeufe
        {
            get
            {
                return _verkaeufe;
            }
            protected set
            {
                _verkaeufe = value;
                OnPropertyChanged("Verkaeufe");
                OnPropertyChanged("Saldo");
                OnPropertyChanged("Einsatz");
            }
        }
        private double _verkaeufe;

        public double Kaeufe
        {
            get
            {
                return _kaeufe;
            }
            protected set
            {
                _kaeufe = value;
                OnPropertyChanged("Kaeufe");
                OnPropertyChanged("Saldo");
                OnPropertyChanged("Einsatz");
            }
        }
        private double _kaeufe;

        public double Dividende
        {
            get
            {
                return _dividende;
            }
            protected set
            {
                _dividende = value;
                OnPropertyChanged("Dividende");
                OnPropertyChanged("Saldo");
                OnPropertyChanged("Einsatz");
            }
        }
        private double _dividende;

        public double Saldo
        {
            get
            {
                return Einsatz * -1;
            }
        }

        public double Einsatz
        {
            get
            {
                return _kaeufe - _verkaeufe - _dividende;
            }
        }

        public double GesamtWert
        {
            get
            {
                return _gesamtWert;
            }
            protected set
            {
                _gesamtWert = value;
                OnPropertyChanged("GesamtWert");
            }
        }
        private double _gesamtWert;

        public double Gewinn
        {
            get
            {
                return _gewinn;
            }
            protected set
            {
                _gewinn = value;
                OnPropertyChanged("Gewinn");
            }
        }
        private double _gewinn;
    }

    public class JahresUebersichtSummaryViewModel : SummaryViewModel
    {
        public override void Update(DataTable dtPosten)
        {
            Verkaeufe = dtPosten.CalculateSum("Verkaeufe");
            Kaeufe = dtPosten.CalculateSum("Kaeufe");
            Dividende = dtPosten.CalculateSum("Dividende");
            GesamtWert = dtPosten.CalculateSum("Gesamtwert");
            Gewinn = GesamtWert + Saldo;
        }
    }

    public class DepotEntwicklungSummaryViewModel : SummaryViewModel
    {
        public override void Update(DataTable dtPosten)
        {
            if (dtPosten.Rows.Count > 0)
            {
                DataRow lastRow = dtPosten.Rows[dtPosten.Rows.Count - 1];
                Verkaeufe = lastRow.GetDoubleValue("Verkaeufe");
                Kaeufe = lastRow.GetDoubleValue("Kaeufe");
                GesamtWert = lastRow.GetDoubleValue("Gesamtwert");
            }
            else
            {
                Verkaeufe = 0.0;
                Kaeufe = 0.0;
                GesamtWert = 0.0;
            }

            Gewinn = GesamtWert + Saldo;
        }
    }

    public class DepotJahresAbschlussSummaryViewModel : SummaryViewModel
    {
        public DepotJahresAbschlussSummaryViewModel()
        {
            _gesamtWertVorjahr = 0.0;
        }

        public override void Update(DataTable dtPosten)
        {
            Verkaeufe = dtPosten.CalculateSum("Verkauf");
            Kaeufe = dtPosten.CalculateSum("Kauf");
            Dividende = dtPosten.CalculateSum("Dividende");
            GesamtWert = dtPosten.CalculateSum("Gesamtwert");
            GesamtWertVorjahr = dtPosten.CalculateSum("Vorjahr");

            Gewinn = GesamtWert - GesamtWertVorjahr + Saldo;
        }

        public override void Reset()
        {
            base.Reset();
            GesamtWertVorjahr = 0.0;
        }

        public double GesamtWertVorjahr
        {
            get
            {
                return _gesamtWertVorjahr;
            }
            private set
            {
                _gesamtWertVorjahr = value;
                OnPropertyChanged("GesamtWertVorjahr");
            }
        }
        private double _gesamtWertVorjahr;
    }

    public class WertpapierEntwicklungSummaryViewModel : SummaryViewModel
    {
        public override void Update(DataTable dtPosten)
        {
            if (dtPosten.Rows.Count > 0)
            {
                DataRow lastRow = dtPosten.Rows[dtPosten.Rows.Count - 1];
                Verkaeufe = lastRow.GetDoubleValue("Verkaeufe");
                Kaeufe = lastRow.GetDoubleValue("Kaeufe");
                GesamtWert = lastRow.GetDoubleValue("Gesamtwert");
            }
            else
            {
                Verkaeufe = 0.0;
                Kaeufe = 0.0;
                GesamtWert = 0.0;
            }

            Gewinn = GesamtWert + Saldo;
        }
    }
}
