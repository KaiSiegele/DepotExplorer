using System.Diagnostics;

namespace VermoegensData
{
    /// <summary>
    /// Hilfsklasse, um alle Initialisierung der Daten durchzuführen
    /// </summary>
    static public class Starter
    {
        /// <summary>
        /// Prueft die Meta-Daten
        /// liest die wichtigsten Stammdaten aus der Datenbank
        /// prüft, ob Daten eingelesen wurden
        /// und initialsiert die Validations
        /// </summary>
        /// <param name="startExploer">Wenn true wird der Explorer gestartet, bei false das Admin-Tool</param>
        /// <param name="message">Fehlermeldung</param>
        /// <returns>Daten erfolgreich initialisiert</returns>
        public static bool InitData(bool startExploer, ref string message)
        {
            Debug.Assert(started == false);
            bool result = false;
            if (MetaData.CheckDatabaseVersion(1, 1, ref message))
            {
                if (MetaData.CheckObjektTypen(ref message))
                {
                    if (startExploer)
                    {
                        if (MasterData.Load(true, ref message))
                        {
                            result = true;
                            started = true;
                            AddValidations();
                        }
                    }
                    else
                    {
                        result = true;
                        started = true;
                        AddValidations();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Liest die Objekte ein, die in den Explorern bearbeitet
        /// und in den Reports ausgewählt werden können.
        /// </summary>
        public static void LoadCaches()
        {
            string message = string.Empty;
            Debug.Assert(started == true);
            CachedData.Load(ref message);
        }

        private static void AddValidations()
        {
            Addresse.AddValidations();
            Bank.AddValidations();
            Depot.AddValidations();
            Fond.AddValidations();
            Aktie.AddValidations();
            Kurs.AddValidations();
            Bestand.AddValidations();
        }

        private static bool started = false;
    }
}
