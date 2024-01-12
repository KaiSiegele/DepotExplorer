using Tools;

namespace Basics
{
    /// <summary>
    /// Hilfsklasse für die Überprüfungen
    /// </summary>
    public static class ValidationTools
    {
        /// <summary>
        /// Prüft, dass nach dem Speichern des geänderten Objekts
        /// in der Liste immer noch alle Namen eindeutig sind.
        /// Falls nicht wird eine Fehlermeldung ausgegeben
        /// </summary>
        /// <param name="boChanged">Geändertes Objekt</param>
        /// <param name="boFound">Gefundenes Objeckt in der Liste</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>Alle Namen sind noch eindeutig</returns>
        public static bool CheckNameUnique(BaseObject boChanged, BaseObject boFound, ref string error)
        {
            bool result = true;
            if (!CheckObjectsUnique(boChanged, boFound))
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("UniqueNameValidation_NotUnique");
            }
            return result;
        }

        /// <summary>
        /// Prüft, dass nach dem Speichern des geänderten Objekts
        /// in der Liste immer noch alle Werte des Felds eindeutig sind.
        /// Falls nicht wird eine Fehlermeldung ausgegeben
        /// </summary>
        /// <param name="boChanged">Geändertes Objekt</param>
        /// <param name="boFound">Gefundenes Objeckt in der Liste</param>
        /// <param name="field">Feldname</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>Alle Wertes des Felds sind noch eindeutig</returns>
        public static bool CheckValueUnique(BaseObject boChanged, BaseObject boFound, string field, ref string error)
        {
            bool result = true;
            if (!CheckObjectsUnique(boChanged, boFound))
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("UniqueValueValidation_NotUnique", field, boFound);
            }
            return result;
        }

        /// <summary>
        /// Prüft, dass nach dem Speichern des geänderten Objekts
        /// in der Liste immer noch alle Objekte eindeutig sind
        /// </summary>
        /// <param name="boChanged">Geändertes Objekt</param>
        /// <param name="boFound">Gefundenes Objeckt in der Liste</param>
        /// <returns>Alle Objekte sind noch eindeutig</returns>
        private static bool CheckObjectsUnique(BaseObject boChanged, BaseObject boFound)
        {
            return (boFound == null ? true : (boChanged.Id == boFound.Id));
        }
    }
}
