using System;
using System.Windows.Data;
using System.Diagnostics;
using System.Globalization;
using Tools;

namespace UserInterface
{
    /// <summary>
    /// Hilfsklasse, um Doubles in Strings umzuwandeln und umgekehrt
    /// Für eine spezielle Formatierung für den Double kann in den abgeleiteten
    /// Klassen die Methode FormatDouble überschrieben werden
    /// </summary>
    public class BaseDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt den übergebenen Double in einen String um
        /// </summary>
        /// <param name="value">Umzuwandelnder Double</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Ländereinstellungen</param>
        /// <returns>String, der einen Double darstellt</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<double, string>(value, targetType))
            {
                return FormatDouble((double)value);
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        /// <summary>
        /// Wandelt den übergebenen String zurück in einen Double um
        /// </summary>
        /// <param name="value">String, der einen Double darstellt</param>
        /// <param name="targetType">Zieltyp (muss double sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Ländereinstellungen</param>
        /// <returns>Aus dem String erzeugter Double oder Binding.Nothing bei Fehlern</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<string, double>(value, targetType))
            {
                try
                {
                    double conversion = System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
                    return conversion;
                }
                catch (Exception ex)
                {
                    Log.Write(TraceLevel.Warning, "BaseConverter", "ConvertBack", ex.Message);
                    return Binding.DoNothing;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        /// <summary>
        /// Erzeugt aus dem übergebenen Double einen String
        /// In abgeleiteten Klassen kann eine andere Formatierung angegeben werden
        /// </summary>
        /// <param name="value">Umzuwandelnder Double</param>
        /// <returns>String, der einen Double gem. der Formatierung darstellt</returns>
        protected virtual string FormatDouble(double value)
        {
            return value.ToString("G", CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Hilfsklasse, um DateTimes in Strings umzuwandeln und umgekehrt
    /// Für eine spezielle Formatierung für den DateTime kann in den abgeleiteten
    /// Klassen die Methode FormatDateTime überschrieben werden
    /// </summary>
    public class BaseDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt den übergebenen DateTime in einen String um
        /// </summary>
        /// <param name="value">Umzuwandelnder DateTime</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Ländereinstellungen</param>
        /// <returns>String, der einen DateTime darstellt</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<DateTime, string>(value, targetType))
            {
                return this.FormatDateTime((DateTime)value);
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        /// <summary>
        /// Wandelt den übergebenen String zurück in einen DateTime um
        /// </summary>
        /// <param name="value">String, der einen DateTime darstellt</param>
        /// <param name="targetType">Zieltyp (muss DateTime sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture">Ländereinstellungen</param>
        /// <returns>Aus dem String erzeugter DateTime oder Binding.Nothing bei Fehlern</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<string, DateTime>(value, targetType))
            {
                try
                {
                    DateTime conversion = System.Convert.ToDateTime(value, CultureInfo.CurrentCulture);
                    return conversion;
                }
                catch (Exception ex)
                {
                    Log.Write(TraceLevel.Warning, "BaseConverter", "ConvertBack", ex.Message);
                    return Binding.DoNothing;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        /// <summary>
        /// Erzeugt aus dem übergebenen DateTime einen String
        /// In abgeleiteten Klassen kann eine andere Formatierung angegeben werden
        /// </summary>
        /// <param name="value">Umzuwandelnder DateTime</param>
        /// <returns>String, der einen DateTime gem. der Formatierung darstellt</returns>
        protected virtual string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("d", CultureInfo.CurrentCulture);
        }
    }
}
