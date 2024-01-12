using Basics;

namespace Persistence
{
    /// <summary>
    /// Basisklasse eines Befehls
    /// Bei der Ausfuehrung eines Befehls werden Informationen
    /// a) aus der Datenbank gelesen und in die Datenbank geschrieben
    /// b) aus uebergebenen Objekten gelesen und in den Objekten abgespeichert
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        /// Gibt eine Beschreibung zurueck
        /// </summary>
        /// <param name="commandTyp">Befehlstyp</param>
        /// <returns>Beschreibung</returns>
        public abstract string GetDescription(CommandTyp commandTyp);

        /// <summary>
        /// Fuehrt einen Befehl aus
        /// Abhaengig vom Befehlstyp werden Daten gelesen oder
        /// veraendert
        /// </summary>
        /// <param name="commandTyp">Befehlstyp</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Befehl erfolgreich ausgefuehrt</returns>
        public abstract bool Execute(CommandTyp commandTyp, Error error);
    }
}