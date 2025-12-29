using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// Klasse, um einzelne Zeilen der DataTable zu verändern
    /// </summary>
    public abstract class DataRowUpdater
    {
        /// <summary>
        /// Ändert die Werte von Spalten in der Zeile
        /// </summary>
        /// <param name="row"Zeile></param>
        public abstract void Update(DataRow row);
    }

    /// <summary>
    /// Hilfreiche Methoden zur Ergaenzung der Klasse DataTable
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Berechnet die Summe der Werte der Spalte aller Zeilen
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="column">Name der aufzuaddierenden Spalte</param>
        /// <returns>Summe der Spaltenwerte, 0.0 bei Fehlern oder leerer Tabelle</returns>
        public static double CalculateSum(this DataTable dt, string column)
        {
            double sum = 0.0;
            try
            {
                sum = (from r in dt.AsEnumerable() select r.Field<double>(column)).Sum();
            }
            catch (Exception ex)
            {
                Log.Write("DataTableExtension", "CalculateColumnSum", ex);
            }
            return sum;
        }

        /// <summary>
        /// Fuegt der Tabelle eine neue Spalte zu
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column">Name der neuen Spalte</param>
        /// <param name="typename">Name des Typs</param>
        /// <returns>Spalte konnte erfolgreich eingefuegt werden</returns>
        public static bool AddColumn(this DataTable dt, string column, string typename)
        {
            bool result = false;
            if (dt.Columns.Cast<DataColumn>().Any(c => c.ColumnName == column) == false)
            {
                try
                {
                    Type columntype = Type.GetType(typename);
                    if (columntype != null)
                    {
                        dt.Columns.Add(column, columntype);
                        result = true;
                    }
                    else
                    {
                        Log.Write(TraceLevel.Error, "DataTableExtension", "AddColumn", "Für Column {0} falschen Typ {1} angegeben", column, typename);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("DataTableExtension", "AddColumn", ex);
                }
            }
            else
            {
                Log.Write(TraceLevel.Error, "DataTableExtension", "AddColumn", "Column {0} bereits vorhanden", column);
            }
            return result;
        }

        /// <summary>
        /// Berechnet fuer jede Zeile die Summe der angegebenen Spalten 
        /// und schreibt sie in die Ergebnisspalte
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="resultcolumn">Name der Ergebnisspalte</param>
        /// <param name="columns">Spalten, deren Wert aufaddiert werden sollen</param>
        public static void UpdateColumnSum(this DataTable dt, string resultcolumn, params string[] columns)
        {
            foreach (DataRow row in dt.Rows)
            {
                UpdateColumnSum(row, resultcolumn, columns);
            }
        }

        /// <summary>
        /// Ändert alle Zeilen in der Tabelle
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="updater">Objekt zum Ändern der Zeilen</param>
        public static void UpdateRows(this DataTable dt, DataRowUpdater updater)
        {
            foreach (DataRow row in dt.Rows)
            {
                updater.Update(row);
            }
        }


        static void UpdateColumnSum(DataRow row, string resultcolumn, params string[] columns)
        {
            double sum = 0.0;
            foreach (string col in columns)
            {
                sum += row.GetDoubleValue(col);
            }
            row.SetDoubleValue(resultcolumn, sum);
        }
    }
}
