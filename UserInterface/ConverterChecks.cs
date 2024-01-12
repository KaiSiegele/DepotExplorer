using System;
using System.Diagnostics;

namespace UserInterface
{
    /// <summary>
    /// Hilfeklasse, um die Typen der Parameter bei den Convertern
    /// zu überprüfen
    /// </summary>
    static public class ConverterChecks
    {
        /// <summary>
        /// Prüft, ob zu konvertierender Wert und Zieltyp mit den erwarteten Typen überein stimmen
        /// </summary>
        /// <typeparam name="T0">Erwarteter Typ des Objekts</typeparam>
        /// <typeparam name="T1">Erwarteter Zieltyp</typeparam>
        /// <param name="value">Zu konvertierender Wert</param>
        /// <param name="targetType">Zieltyp</param>
        /// <returns>Zu konvertierender Wert und Zieltyp stimmen mit den erwarteten Typen überein</returns>
        public static bool CheckTypes<T0, T1>(object value, Type targetType)
        {
            return CheckTypes(value, targetType, typeof(T0), typeof(T1));
        }

        /// <summary>
        /// Prüft, ob zu konvertierender Wert und Zieltyp mit den erwarteten Typen überein stimmen
        /// </summary>
        /// <param name="value">Zu konvertierender Wert</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="expecteObjectType">Erwarteter Typ des Objekts</param>
        /// <param name="expectedTargetType">Erwarteter Zieltyp</param>
        /// <returns>Zu konvertierender Wert und Zieltyp stimmen mit den erwarteten Typen überein</returns>
        public static bool CheckTypes(object value, Type targetType, Type expecteObjectType, Type expectedTargetType)
        {
            if (value != null)
            {
                return CheckType(true, value.GetType(), expecteObjectType) && CheckType(false, targetType, expectedTargetType);
            }
            else
            {
                Debug.Assert(false, "null-Wert nicht erlaubt.");
                return false;
            }
        }

        private static bool CheckType(bool source, Type type, Type expectedType)
        {
            if (type != expectedType && type != typeof(object))
            {
                string message = string.Format("Falscher {0} (erwartet {1}, bekommen {2}).", source ? "Quelltype" : "Zieltype", type.Name, expectedType.Name);
                Debug.Assert(false, message);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
