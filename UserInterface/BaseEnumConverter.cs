using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace UserInterface
{
    /// <summary>
    /// Klasse um einen Enum-Typen in die zugehörige Beschreibung umzuwandeln und umgekehrt
    /// </summary>
    /// <typeparam name="T">Enum-Typ</typeparam>
    public class BaseEnumConverter<T> : IValueConverter where T : struct
    {
        /// <summary>
        /// Wandelt den Enum-Typ in die Beschreibung um.
        /// Für den  default-Wert des Enum-Tys wird "   " zurückgeliefert.
        /// </summary>
        /// <param name="value">Enum-Type</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Beschreibung des Enum-Typs</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string description;

            if (ConverterChecks.CheckTypes(value, targetType, typeof(T), typeof(string)))
            {
                T result;
                if (Enum.TryParse<T>(value.ToString(), out result))
                {
                    if (result.Equals(default(T)))
                    {
                        description = defaultDescription;
                    }
                    else
                    {
                        description = result.ToString();
                    }
                }
                else
                {
                    description = defaultDescription;
                }
            }
            else
            {
                description = string.Empty;
            }
            return description;
        }

        /// <summary>
        /// Wandelt eine Beschreibung zurück in den Enum-Typ
        /// Falls es keinen Enum-Typen zur Beschreibung gibt,
        /// wird der default-Wert des Enum-Typen zurückgeliefert
        /// </summary>
        /// <param name="value">Beschreibung</param>
        /// <param name="targetType">>Zieltyp (muss enum sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Ermittelter Enum-Typ</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            T result;
            if (ConverterChecks.CheckTypes(value, targetType, typeof(string), typeof(T)))
            {
                string descripton = value as string;
                if (descripton != null)
                {
                    if (descripton == defaultDescription)
                    {
                        result = default(T);
                    }
                    else
                    {
                        if (!Enum.TryParse<T>(descripton, out result))
                        {
                            result = default(T);
                        }
                    }
                }
                else
                {
                    result = default(T);
                }
            }
            else
            {
                result = default(T);
            }
            return result;
        }

        readonly static string defaultDescription = "   ";
    }
}
