using Basics;
using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace UserInterface
{
    /// <summary>
    /// Hilfsklasse, um die Ids von BaseObjects in Namen zu konvertieren 
    /// Die BaseObjects deren Ids konvertiert werden sollen, werden im 
    /// Feld BaseObjects abgespeichert. 
    /// Die Ids können um BaseObject.All erweitert werden, dieser Wert
    /// wird in den String "Alle" konvertiert. Dazu muss AddAll auf true
    /// gesetzt werden.
    /// </summary>
    public class BaseObjectConverter : IValueConverter
    {
        public static readonly int All = -2;
        public static readonly int NoSelection = -1;

        /// <summary>
        /// Konvertiert die übergebene Id des BaseObjects in den zugehörigen Namen
        /// </summary>
        /// <param name="value">Id</param>
        /// <param name="targetType">Zieltyp (muss string sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Name des BaseObjects</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string description;
            if (ConverterChecks.CheckTypes<int, string>(value, targetType))
            {
                int id = (int)value;
                if (AddAll && id == All)
                {
                    description = Properties.Resource.All;
                }
                else if (AddNotSpecified && id == BaseObject.NotSpecified)
                {
                    description = notSpecifiedDescription;
                }
                else
                {
                    description = _bos.GetObjectName(id);
                }
            }
            else
            {
                description = string.Empty;
            }
            return description;
        }

        /// <summary>
        /// Konvertiert den übergebenen Name des BaseObjects in die zugehörige Id
        /// </summary>
        /// <param name="value">Name</param>
        /// <param name="targetType">Zieltyp (muss int sein)</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Id des BaseObjects</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int id;
            if (ConverterChecks.CheckTypes<string, int>(value, targetType))
            {
                string name = value.ToString();
                if (AddAll && name == Properties.Resource.All)
                    id = All;
                else if (AddNotSpecified && name == notSpecifiedDescription)
                    id = BaseObject.NotSpecified;
                else
                {
                    id = _bos.GetObjectId(name);
                    return id == BaseObject.NotSpecified ? NoSelection : id;
                }
            }
            else
            {
                id = NoSelection;
            }
            return id;
        }

        /// <summary>
        /// Fügt den string "Alle" vorne an die Liste der übergebenen Namen an
        /// </summary>
        /// <param name="objectNames">Namen der Objekte</param>
        /// <returns>String "Alle" und die Liste der Namen</returns>
        public static List<string> GetObjectNamesWithAll(IEnumerable<string> objectNames)
        {
            return GetObjectNamesWithAdditionn(objectNames, Properties.Resource.All);
        }

        /// <summary>
        /// Fügt den string notSpecifieldtDescription vorne an die Liste der übergebenen Namen an
        /// </summary>
        /// <param name="objectNames">Namen der Objekte</param>
        /// <returns>String notSpecifieldtDescription und die Liste der Namen</returns>

        public static List<string> GetObjectNamesWithNotSpecified(IEnumerable<string> objectNames)
        {
            return GetObjectNamesWithAdditionn(objectNames, notSpecifiedDescription);
        }

        protected BaseObjects BaseObjects
        {
            set
            {
                _bos = value;
            }
        }

        protected bool AddAll { private get; set; }       
        protected bool AddNotSpecified { private get; set; }

        private static List<string> GetObjectNamesWithAdditionn(IEnumerable<string> objectNames, string addition)
        {
            List<string> objectNamesWithAll = new List<string> { addition };
            objectNamesWithAll.AddRange(objectNames);
            return objectNamesWithAll;
        }

        private BaseObjects _bos = null;
        readonly static string notSpecifiedDescription = "  ";
    }
}
