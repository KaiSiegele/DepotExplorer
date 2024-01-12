using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tools
{
    public static class Misc
    {
        /// <summary>
        /// Erstellt eine lesbare Beschreibung aus der Liste von Object-Namen
        /// </summary>
        /// <param name="objectNames">>Liste von Object-Namen</param>
        /// <param name="delimiter">Trennzeichen</param>
        /// <returns>Erstellte Beschreibung</returns>
        public static string GetNamesAsString(IEnumerable<string> objectNames, string delimiter = ", ")
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string objectName in objectNames)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(delimiter);
                }
                sb.Append(objectName);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Erstellt eine lesbare Beschreibung aus der Liste von Ids
        /// </summary>
        /// <param name="ids">Liste von Ids</param>
        /// <param name="delimiter">Trennzeichen</param>
        /// <returns>Erstellte Beschreibung</returns>
        public static string GetIdsAsString(IEnumerable<int> ids, string delimiter = ", ")
        {
            return GetNamesAsString(from id in ids select id.ToString(), delimiter);
        }

        /// <summary>
        /// Erstellt eine lesbare Beschreibung aus der Liste von Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="numberOfIds">Anzahl der Ids, die ausgegeben werden sollen</param>
        /// <param name="abbreviation">Abkürzung für die übrigen</param>
        /// <param name="delimiter">Trennzeichen</param>
        /// <returns>Erstellte Beschreibung</returns>
        public static string GetIdsAsString(IEnumerable<int> ids, int numberOfIds, string abbreviation = "...", string delimiter = ", ")
        {
            Debug.Assert(numberOfIds > 0);
            if (ids.Count() > numberOfIds)
            {
                return string.Format("{0} {1}", GetIdsAsString(ids.Take(numberOfIds), delimiter), abbreviation);
            }
            else
            {
                return GetIdsAsString(ids, delimiter);
            }
        }

        /// <summary>
        /// Gibt eine lesbare Beschreibung der Anzahl der Elemente zurück
        /// </summary>
        /// <param name="number">Anzahl der Elemente</param>
        /// <param name="descSingular">Beschreibung in der Einzahl</param>
        /// <param name="descPlural">Beschreibung in der Mehrzahl</param>
        /// <returns>Erzeugte Beschreibung</returns>
        public static string GetNumberDescription(int number, string descSingular, string descPlural)
        {
            string desc = (number == 1 ? descSingular : descPlural);
            return string.Format("{0} {1}", number, desc);
        }

        /// <summary>
        /// Gibt aus der Liste der übergebenen Werte alle zurück,
        /// die mindestens zweimal vorhanden sind
        /// </summary>
        /// <param name="values"></param>
        /// <param name="doubleValues"></param>
        /// <returns>Mindestens ein Wert war, mehr als einmal vorhanden</returns>
        public static bool GetDoubles(IEnumerable<int> values, List<int> doubleValues)
        {
            Debug.Assert(doubleValues.Count == 0);
            Dictionary<int, bool> usedValues = new Dictionary<int, bool>();
            foreach (var value in values)
            {
                bool used;
                if (usedValues.TryGetValue(value, out used))
                {
                    if (!doubleValues.Contains(value))
                    {
                        doubleValues.Add(value);
                    }
                }
                else
                {
                    usedValues.Add(value, true);
                }
            }
            return (doubleValues.Count > 0);
        }

        /// <summary>
        /// Erzeugt einen Dateinamen, der das Datum der Erstellung enthält
        /// </summary>
        /// <param name="praefix">Präfix</param>
        /// <param name="suffix">Dateiendung</param>
        /// <returns>Dateiname</returns>
        public static string CreateFilename(string praefix, string suffix)
        {
            DateTime now = DateTime.Now;
            string filename = string.Format("{0}{1:0000}{2:00}{3:00}.{4}", praefix, now.Year, now.Month, now.Day, suffix);
            return filename;
        }
    }
}
