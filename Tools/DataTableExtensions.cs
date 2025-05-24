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
        /// Fuehrt fuer jede Zeile mit den Werten der Spalten die uebergebene
        /// Funktion aus und speichert das Ergebnis in der Ergebnisspalte ab
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="resultcolumn">Name der Ergebnisspalte</param>
        /// <param name="column1">Name der 1. Spalte</param>
        /// <param name="column2">Name der 2. Spalte</param>
        /// <param name="column3">Name der 3. Spalte</param>
        /// <param name="func">Auszufuehrende Funktion</param>
        public static void UpdateColumn(this DataTable dt, string resultcolumn, string column1, string column2, string column3, Func<double, double, double, double> func)
        {
            foreach (DataRow row in dt.Rows)
            {
                UpdateColumn(row, resultcolumn, column1, column2, column3, func);
            }
        }

        /// <summary>
        /// Fuehrt fuer jede Zeile mit den Werten der Spalten die uebergebene
        /// Funktion aus und speichert das Ergebnis in der Ergebnisspalte ab
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="resultcolumn">Name der Ergebnisspalte</param>
        /// <param name="column1">Name der 1. Spalte</param>
        /// <param name="column2">Name der 2. Spalte</param>
        /// <param name="func">Auszufuehrende Funktion</param>
        public static void UpdateColumn(this DataTable dt, string resultcolumn, string column1, string column2, Func<double, double, double> func)
        {
            foreach (DataRow row in dt.Rows)
            {
                UpdateColumn(row, resultcolumn, column1, column2, func);
            }
        }

        /// <summary>
        /// Fuehrt fuer jede Zeile mit den Werten der Spalten die uebergebene
        /// Funktion aus und speichert das Ergebnis in der Ergebnisspalte ab
        /// </summary>
        /// <param name="dt">Tabelle</param>
        /// <param name="resultcolumn">Name der Ergebnisspalte</param>
        /// <param name="column1">Name der 1. Spalte</param>
        /// <param name="column2">Name der 2. Spalte</param>
        /// <param name="column3">Name der 3. Spalte</param>
        /// <param name="column4">Name der 4. Spalte</param>
        /// <param name="func">Auszufuehrende Funktion</param>
        public static void UpdateColumn(this DataTable dt, string resultcolumn, string column1, string column2, string column3, string column4, Func<double, double, double, double, double> func)
        {
            foreach (DataRow row in dt.Rows)
            {
                UpdateColumn(row, resultcolumn, column1, column2, column3, column4, func);
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

        static void UpdateColumn(DataRow row, string resultcolumn, string column1, string column2, Func<double, double, double> func)
        {
            double value1 = row.GetDoubleValue(column1);
            double value2 = row.GetDoubleValue(column2);
            row.SetDoubleValue(resultcolumn, func(value1, value2));
        }

        static void UpdateColumn(DataRow row, string resultcolumn, string column1, string column2, string column3, Func<double, double, double, double> func)
        {
            double value1 = row.GetDoubleValue(column1);
            double value2 = row.GetDoubleValue(column2);
            double value3 = row.GetDoubleValue(column3);
            row.SetDoubleValue(resultcolumn, func(value1, value2, value3));
        }

        static void UpdateColumn(DataRow row, string resultcolumn, string column1, string column2, string column3, string column4, Func<double, double, double, double, double> func)
        {
            double value1 = row.GetDoubleValue(column1);
            double value2 = row.GetDoubleValue(column2);
            double value3 = row.GetDoubleValue(column3);
            double value4 = row.GetDoubleValue(column4);
            row.SetDoubleValue(resultcolumn, func(value1, value2, value3, value4));
        }
    }
}
