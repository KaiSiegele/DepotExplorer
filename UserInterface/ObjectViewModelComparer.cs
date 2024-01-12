using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace UserInterface
{
    /// <summary>
    /// Klasse um zwei ObjectViewModels miteinander zu vergleichen
    /// </summary>
    internal class ObjectViewModelComparer : IComparer<ObjectViewModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortCriterium">Sortierungskriterium</param>
        public ObjectViewModelComparer(string sortCriterium)
        {
            SortCriterium = sortCriterium;
            SortAscending = true;
        }

        /// <summary>
        /// Vergleicht zwei ObjectViewModels miteinander
        /// Die Methode 
        /// <0 wenn erstes ObjectViewModel kleiner als zweites
        /// 0 wenn beide ViewModels gleich
        /// >0 wenn erstes ViewModel größer als zweites
        /// </summary>
        /// <param name="firstOvm">Erstes ObjectViewMode</param>
        /// <param name="secondOvm">Zweites ObjectViewModel</param>
        /// <returns>Vergleichsergebnis</returns>
        public int Compare(ObjectViewModel firstOvm, ObjectViewModel secondOvm)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(SortCriterium), "Keine Sortierung gesetzt");

            Func<ObjectViewModel, string, IComparable> getComparableValue =
                delegate (ObjectViewModel bvm, string field)
                {
                    IComparable result;

                    try
                    {
                        PropertyInfo pi = bvm.GetType().GetProperty(field);
                        result = pi.GetValue(bvm, null) as IComparable;
                    }
                    catch (Exception ex)
                    {
                        Log.Write("BaseViewModelComparer", "Compare", ex);
                        result = null;
                    }
                    Debug.Assert(!(result == null), string.Format("Cannot get value from {0}", bvm));

                    return result;
                };

            IComparable firstValue = getComparableValue(firstOvm, SortCriterium);
            IComparable secondValue = getComparableValue(secondOvm, SortCriterium);

            if (firstValue != null && secondValue != null)
            {
                return firstValue.CompareTo(secondValue) * (SortAscending ? 1 : -1);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Aktualisiert das Sortierungskriterium
        /// Falls nach dem übergebenen Kriterium bereits sortiert wird,
        /// wird die Richtung der Sortierung geändert
        /// </summary>
        /// <param name="sortCriterium">Neue Sortierungskriterium</param>
        public void UpdateSortCriterium(string sortCriterium)
        {
            if (SortCriterium == sortCriterium)
            {
                SortAscending = ! SortAscending;
            }
            else
            {
                SortCriterium = sortCriterium;
                SortAscending = true;
            }
        }
        public string SortCriterium { private set; get; }
        public bool SortAscending { private set; get; }
    }
}