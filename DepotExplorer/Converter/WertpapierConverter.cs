using Basics;
using System;
using System.Globalization;
using System.Windows.Data;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Hilfeklasse, um die Ids von Wertpapieren in Namen zu konvertieren 
    /// </summary>
    class WertpapierConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert die übergebene Id des Wertpapiers in den zugehörigen Namen
        /// </summary>
        /// <param name="value">Id</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Name des Wertpapiers</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string description = string.Empty;
            if (ConverterChecks.CheckTypes<int, string>(value, targetType))
            {
                Wertpapier wp = WertpapierInfo.GetWertpapierById((int)value);
                if (wp != null)
                {
                    description = wp.Name;
                }
            }
            return description;
        }

        /// <summary>
        /// Konvertiert den übergebenen Name des Wertpapiers in die zugehörige Id
        /// </summary>
        /// <param name="value">Name</param>
        /// <param name="targetType">Zieltyp (muss int sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Id des Wertpapiers</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = BaseObject.NotSpecified;
            if (ConverterChecks.CheckTypes<string, int>(value, targetType))
            {
                Wertpapier wp = WertpapierInfo.GetWertpapierByName(value.ToString());
                if (wp != null)
                {
                    id = wp.Id;
                }
            }
            return id;
        }
    }
}
