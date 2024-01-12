using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Tools;
using UserInterface;

namespace DepotExplorer
{
    /// <summary>
    /// Klasse für die Umwandlung eines Doubles in eine Farbe
    /// Wert < o = Grün
    /// Wert = 0 = Schwarz
    /// Wert > 0 = Rot
    /// </summary>
    public class SaldoColourConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt einen Double in eine Farbe um
        /// </summary>
        /// <param name="value">Doublewert</param>
        /// <param name="targetType">Zieltyp (muss Brush sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Ermittelte Farbe wie beschrieben</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<double, Brush>(value, targetType))
            {
                try
                {
                    double saldo = System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
                    if (saldo > 0.0)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }
                    else if (saldo < 0.0)
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Black);
                    }

                }
                catch (Exception ex)
                {
                    Log.Write(TraceLevel.Verbose, "SaldoColourConverter", "BestandQuery", "Convert", ex.Message);
                    return new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Klasse für die Umwandlung eines Doubles in eine Beschreibung
    /// Wert < o = Negativ
    /// Wert = 0 = Null
    /// Wert > 0 = Positiv
    /// Anhand dieser Beschreibung wird anschließend die Farbe für die Anzeige ermittelt
    /// </summary>
    public class SaldoStringConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt einen Double in eine Beschreibung um
        /// </summary>
        /// <param name="value">Double-Wert</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Ermittelte Beschreibung wie beschrieben</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ConverterChecks.CheckTypes<double, string>(value, targetType))
            {
                try
                {
                    double saldo = System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
                    if (saldo > 0.0)
                    {
                        return "Positiv";
                    }
                    else if (saldo < 0.0)
                    {
                        return "Negativ";
                    }
                    else
                    {
                        return "Null";
                    }

                }
                catch (Exception ex)
                {
                    Log.Write(TraceLevel.Verbose, "SaldoStringConverter", "BestandQuery", "Convert", ex.Message);
                    return "Null";
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
