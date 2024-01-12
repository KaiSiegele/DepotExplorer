using System.Collections.Generic;
using System.Linq;
using Basics;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Wert eines Wertpapiers zu einem bestimmten Stichtag (Jahr)
    /// </summary>
    public class Kurs : BaseObject
    {
        public static void AddValidations()
        {
            BaseValidations.AddValidation<Kurs>(new LinkValidation("Wertpapier"));
            BaseValidations.AddValidation<Kurs>(new LinkValidation("Jahr"));
            BaseValidations.AddValidation<Kurs>(new DoubleRangeValidation("Wert", 0.0, false, false));
        }

        internal static Kurs CreateKurs(int wertpapierid, int jahrid, double wert)
        {
            Kurs kurs = BaseObject.CreateDefault<Kurs>(ObjectState.Inserted);
            kurs.Id = MetaData.GetNextObjectId(MetaData.ObjektTypID.Kurs);
            kurs.Name = string.Empty;
            kurs.Jahr = jahrid;
            kurs.Wertpapier = wertpapierid;
            kurs.Wert = wert;
            return kurs;
        }

        public Kurs()
        {
            Name = string.Empty;
            Jahr = BaseObject.NotSpecified;
            Wertpapier = BaseObject.NotSpecified;
            Wert = 0.0;
        }

        public override string ToString()
        {
            return string.Format("Kurs ({0}, {1}, {2})", Jahr, Wertpapier, Wert);
        }

        public int Jahr { get; set; }
        public int Wertpapier { get; set; }
        public double Wert { get; set; }
    }

    public class Kurse : BaseObjects
    {
        public override string ToString()
        {
            return string.Format("{0} Kurse", Count);
        }

        public bool InsertKurs(int wertpapierid, int jahrid, ref int id)
        {
            bool result = false;
            Kurs kurs = Kurs.CreateKurs(wertpapierid, jahrid, 0.0);
            if (InsertObject(kurs))
            {
                result = true;
                id = kurs.Id;
            }
            return result;
        }

        public bool RemoveKurs(int id)
        {
            return SetObjectAsRemoved(id);
        }

        /// <summary>
        /// Gibt alle Kurse zurück, die einem Wertpapier mit der
        /// übergebenen Wertpapier-Art zugeorndet sind
        /// </summary>
        /// <param name="wpa">Wertpapier-Art</param>
        /// <returns>Alle gewünschten Kurse</returns>
        public IEnumerable<Kurs> GetKurseOfWertpapierArt(WertpapierArt wpa)
        {
            return (from bo in Values.OfType<Kurs>() where bo.Removed == false && WertpapierInfo.GetWertpapierArt(bo.Wertpapier) == wpa select bo);
        }

        /// <summary>
        /// Fügt für die fehlenden Jahre Kurse hinzu
        /// PC: Alle bisherigen Kurse haben das gleiche Wertpapier
        /// </summary> 
        /// <param name="wertpapierid">Id des Wertpapiers</param>
        /// <param name="jahre">Liste der Jahre</param>
        public void InsertKurseForMissingJahre(int wertpapierid, BaseObjects jahre)
        {
            foreach (var jahr in jahre)
            {
                Kurs kurs = GetFirstObject<Kurs>(k => k.Jahr == jahr.Value.Id && k.Wertpapier == wertpapierid && k.Removed == false);
                if (kurs == null)
                {
                    int id = 0;
                    InsertKurs(wertpapierid, jahr.Value.Id, ref id);
                }
            }
        }

        /// <summary>
        /// Prüft, dass es für jedes Jahr einen Kurs gibt
        /// PC: Alle Kurse haben das gleiche Wertpapier und ein eindeutiges Jahr
        /// </summary>
        /// <returns>true, wenn die Liste der Kurse lückenlos ist, false sonst</returns>
        public bool CheckJahreLueckenlos()
        {
            bool result = true;
            IEnumerable<Kurs> kurse = (from bo in Values.OfType<Kurs>() where bo.Removed == false select bo);
            int anzahl = kurse.Count();
            if (anzahl > 0)
            {
                int minJahr = kurse.Min(k => k.Jahr);
                int maxJahr = kurse.Max(k => k.Jahr);
                result = (maxJahr - minJahr + 1 == anzahl);
            }
            return result;
        }

        /// <summary>
        /// Prüft, dass alle Kurse ein eindeutiges Jahr haben
        /// PC: Alle Kurse haben das gleiche Wertpapier
        /// </summary>
        /// <param name="doubleJahrIds">Liste von Jahren, die von mehreren Kursen benutzt werden</param>
        /// <returns>true wenn alle Kurse ein anderes Jahr haben, false sonst</returns>
        public bool CheckJahreUnique(List<int> doubleJahrIds)
        {
            IEnumerable<int> jahrIds = (from kurs in Values.OfType<Kurs>() where kurs.Removed == false select kurs.Jahr);
            return (Misc.GetDoubles(jahrIds, doubleJahrIds) == false);
        }

        /// <summary>
        /// Prüft, dass es keine Kurse mit doppelten Wertpapieren gibt
        /// PC: Alle Kurse haben das gleiche Jahr
        /// </summary>
        /// <param name="doubleWertpapierIds">Liste von Wertpapieren, die von mehreren Kursen benutzt werden</param>
        /// <returns>true wenn alle Kurse ein anderes Wertpapier haben, false sonst</returns>
        public bool CheckWertpapiereUnique(List<int> doubleWertpapierIds)
        {
            IEnumerable<int> wertpapierIds = (from kurs in Values.OfType<Kurs>() where kurs.Removed == false select kurs.Wertpapier);
            return (Misc.GetDoubles(wertpapierIds, doubleWertpapierIds) == false);
        }
    }
}
