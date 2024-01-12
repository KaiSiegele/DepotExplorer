using System;
using Basics;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;

namespace UserInterface
{
    /// <summary>
    /// Hilfsfunktionen zum Lesen und Beschreiben von Wpf-Controls
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// Gibt das ausgewählte Objekt der ListView zurück.
        /// Die Klasse des Objekts muss von BaseObject abgeleitet sein.
        /// </summary>
        /// <typeparam name="T">Von BaseObject abgeleitete Klasse</typeparam>
        /// <param name="listView">Listview</param>
        /// <returns>Ausgewähltes Objekt oder null, wenn nichts ausgewählt ist oder die Auswahl vom falschen Typ ist</returns>
        public static T GetSelectedBaseObject<T>(ListView listView) where T : BaseObject
        {
            return ToBaseObject<T>(listView.SelectedItem);
        }

        /// <summary>
        /// Gibt das ausgewählte Objekt der Listbox zurück.
        /// Die Klasse des Objekts muss von BaseObject abgeleitet sein.
        /// </summary>
        /// <typeparam name="T">Von BaseObject abgeleitete Klasse</typeparam>
        /// <param name="listBox">Listbox</param>
        /// <returns>Ausgewähltes Objekt oder null, wenn nichts ausgewählt ist oder die Auswahl vom falschen Typ ist</returns>
        public static T GetSelectedBaseObject<T>(ListBox listBox) where T : BaseObject
        {
            return ToBaseObject<T>(listBox.SelectedItem);
        }

        /// <summary>
        /// Gibt das ausgewählte Objekt des DataGrid's zurück.
        /// Die Klasse des Objekts muss von BaseObject abgeleitet sein.
        /// </summary>
        /// <typeparam name="T">Von BaseObject abgeleitete Klasse</typeparam>
        /// <param name="dataGrid">Datagrid</param>
        /// <returns>Ausgewähltes Objekt oder null, wenn nichts ausgewählt ist oder die Auswahl vom falschen Typ ist</returns>
        public static T GetSelectedBaseObject<T>(DataGrid dataGrid) where T : BaseObject
        {
            return ToBaseObject<T>(dataGrid.SelectedItem);
        }

        /// <summary>
        /// Füllt die Combobox mit allen Werten der Enumeration
        /// </summary>
        /// <typeparam name="T">Enumeration</typeparam>
        /// <param name="comboBox">Combobox</param>
        /// <param name="addDefaultValue">Zeigt an, ob fuer den Default-Wert der Enumeration eine Auswahl eingefügt werden soll</param>
        public static void FillCombox<T>(ComboBox comboBox, bool addDefaultValue) where T : struct
        {
            comboBox.Items.Clear();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (item.Equals(default(T)))
                {
                    if (addDefaultValue)
                    {
                        comboBox.Items.Add("   ");
                    }
                }
                else
                {
                    comboBox.Items.Add(item.ToString());
                }
            }
        }

        /// <summary>
        /// Setzt im Datagrid die Eigenschaft der Spalte,
        /// ob sie nur gelesen werden darf
        /// </summary>
        /// <param name="datagrid">Datagrid</param>
        /// <param name="columnName">Name der Spalte</param>
        /// <param name="isReadOnly">true wenn die Spalte nur gelesen werden darf, false sonst</param>
        public static void SetColumnReadOnly(DataGrid datagrid, string columnName, bool isReadOnly)
        {
            DataGridColumn column = datagrid.Columns.Where(h => h.Header.ToString() == columnName).FirstOrDefault();
            if (column != null)
            {
                column.IsReadOnly = isReadOnly;
            }
            else
                Debug.Assert(false);
        }
 
        private static T ToBaseObject<T>(object item) where T : BaseObject
        {
            T tbo = null;
            if (item != null)
            {
                if (item is ObjectViewModel)
                {
                    BaseObject bo = (item as ObjectViewModel).BaseObject;
                    tbo = bo as T;
                }
            }
            return tbo;
        }
    }
}