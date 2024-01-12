using System.Collections.Generic;
using System.Linq;
using Basics;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Käufe. Verkäufe, Dividende und Anzahl von Wertpieren in einem Depot zu einem Stichtag
    /// </summary>
    public class Bestand : BaseObject
    {
        public static void AddValidations()
        {
            BaseValidations.AddValidation<Bestand>(new LinkValidation("Depot"));
            BaseValidations.AddValidation<Bestand>(new LinkValidation("Jahr"));
            BaseValidations.AddValidation<Bestand>(new LinkValidation("Wertpapier"));
        }

        public Bestand()
        {
            Id = BaseObject.NotSpecified;
            Depot = BaseObject.NotSpecified;
            Jahr = BaseObject.NotSpecified;
            Wertpapier = BaseObject.NotSpecified;
            Anteile = 0.0;
            Kauf = 0.0;
            Verkauf = 0.0;
            Dividende = 0.0;
        }

        internal static Bestand CreateBestand(int depotid, int wertpapierid, int jahrid)
        {
            Bestand bestand = BaseObject.CreateDefault<Bestand>(ObjectState.Inserted);
            bestand.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Bestand);
            bestand.Depot = depotid;
            bestand.Jahr = jahrid;
            bestand.Wertpapier = wertpapierid;
            bestand.Anteile = 0.0;
            bestand.Kauf = 0.0;
            bestand.Verkauf = 0.0;
            bestand.Dividende = 0.0;
            return bestand;
        }

        public override string ToString()
        {
            return string.Format("Bestand {0}, {1}, {2}, {3}", Depot, Jahr, Wertpapier, Anteile);
        }

        public int Jahr { get; set; }
        public int Depot { get; set; }
        public int Wertpapier { get; set; }
        public double Anteile { get; set; }
        public double Kauf { get; set; }
        public double Verkauf { get; set; }
        public double Dividende { get; set; }
    }

    public class Bestaende : BaseObjects
    {
        public Bestaende()
          : base()
        {
        }
        public override string ToString()
        {
            return string.Format("{0} Bestaende", Count);
        }


        /// <summary>
        /// Prüft, ob es zu einem Wertpapier Bestaende gibt
        /// </summary>
        /// <param name="wertpapierid">Id des Wertpapiers</param>
        /// <returns>true wenn Bestaende vorhanden, false sonst</returns>
        public bool HasWertpapierBestaende(int wertpapierid)
        {
            return HasObject<Bestand>(b => !b.Removed && b.Wertpapier == wertpapierid);
        }

        /// <summary>
        /// Fügt einen neuen Bestand noch ohne Wertpapier Id ein
        /// </summary>
        /// <param name="depotid">Id des Depots</param>
        /// <param name="jahrid">Id des Jahres</param>
        /// <param name="id">Id des neuen Bestands</param>
        /// <returns>true wennn einfügen erfolgreich, false sonst</returns>
        public bool InsertBestand(int depotid, int jahrid, ref int id)
        {
            return InsertBestand(depotid, jahrid, BaseObject.NotSpecified, ref id);
        }

        public bool RemoveBestand(int id)
        {
            return SetObjectAsRemoved(id);
        }

        /// <summary>
        /// Fügt für alle übergebenen Wertpapier-Ids falls nicht vorhanden
        /// einen neuen Bestand ein
        /// </summary>
        /// <param name="depotid">Id des Depots</param>
        /// <param name="jahrid">Id des Jahres</param>
        /// <param name="wpIds">Ids der Wertpapiere</param>
        public void InsertBestaendeForMissingWertpapiere(int depotid, int jahrid, IEnumerable<int> wpIds)
        {
            foreach (var wpId in wpIds)
            {
                if (!HasObject<Bestand>(b => b.Wertpapier == wpId && b.Depot == depotid && b.Jahr == jahrid && b.Removed == false))
                {
                    int id = 0;
                    InsertBestand(depotid, jahrid, wpId, ref id);
                }
            }
        }

        /// <summary>
        /// Überprüft für alle Jahre, dass es keine doppelten Wertpapiere gibt
        /// </summary>
        /// <param name="doppelteJahrIds">Liste von Jahre, die von mehreren Wertpapieren benutzt werden</param>
        /// <returns>true wenn alle Wertpapiere ein anderes Jahr haben, false sonst</returns>
        public bool CheckWertpapiereUnique(List<int> doppelteJahrIds)
        {
            bool result = true;
            IEnumerable<int> jahrids = (from bo in Values.OfType<Bestand>() where bo.Removed == false select bo.Jahr).Distinct();

            foreach (var jahrId in jahrids)
            {
                if (!CheckWertpapiereProJahrUnique(jahrId))
                {
                    doppelteJahrIds.Add(jahrId);
                    result = false;
                }
            }
            return result;

        }

        private bool InsertBestand(int depotid, int jahrid, int wertpapierid, ref int id)
        {
            bool result = false;
            Bestand bestand = Bestand.CreateBestand(depotid, wertpapierid, jahrid);
            if (InsertObject(bestand))
            {
                result = true;
                id = bestand.Id;
            }
            return result;
        }

        private bool CheckWertpapiereProJahrUnique(int jahr)
        {
            List<int> doppelteWertpapiere = new List<int>();
            IEnumerable<int> wertpapierIds = (from bo in Values.OfType<Bestand>() where bo.Removed == false && bo.Jahr == jahr select bo.Wertpapier);
            return Misc.GetDoubles(wertpapierIds, doppelteWertpapiere) == false;
        }
    }
}
