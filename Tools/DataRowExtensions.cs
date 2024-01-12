using System;
using System.Data;
using System.Diagnostics;

namespace Tools
{
    /// <summary>
    /// Hilfreiche Methoden zur Ergaenzung der Klasse
    /// DataRow, vorwiegend um ohne Exception den Wert
    /// in einer Spalte zu setzen. Beim Fehlerfall wird
    /// eine Fehlermeldung in die Log-Datei geschrieben
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Liesst aus der angegebenen Spalte den Wert als Double-Wert
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="defValue">Wert der verwendet wird, falls Spalte nicht vorhanden oder der Datentyp falsch ist</param>
        /// <returns>Ermittelter Wert</returns>
        static public double GetDoubleValue(this DataRow row, string field, double defValue = 0.0)
        {
            double value = defValue;
            try
            {
                value = row.Field<double>(field);
            }
            catch (Exception ex)
            {
                TraceError("GetDoubleValue", ex);
            }
            return value;
        }

        /// <summary>
        /// Liesst aus der angegebenen Spalte den Wert als Datum
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <returns>Ermittelter Wert, falls Spalte nicht vorhanden oder der Datentyp falsch ist, wird das Tagesdatum zurückgegeben</returns>
        static public DateTime GetDateValue(this DataRow row, string field)
        {
            return GetDateValue(row, field, DateTime.Today);
        }

        /// <summary>
        /// Liest aus der angegebenen Spalte den Wert als Datum
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="defValue">Wert der verwendet wird, falls Spalte nicht vorhanden oder der Datentyp falsch ist</param>
        /// <returns>Ermittelter Wert</returns>
        static public DateTime GetDateValue(this DataRow row, string field, DateTime defValue)
        {
            DateTime value = defValue;
            try
            {
                value = row.Field<DateTime>(field);
            }
            catch (Exception ex)
            {
                TraceError("GetDateValue", ex);
            }
            return value;
        }

        /// <summary>
        /// Liest aus der angegebenen Spalte den Wert als Integer-Wert
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="defValue">Wert der verwendet wird, falls Spalte nicht vorhanden oder der Datentyp falsch ist</param>
        /// <returns>Ermittelter Wert</returns>
        static public int GetIntValue(this DataRow row, string field, int defValue = 0)
        {
            int value = defValue;
            try
            {
                value = row.Field<int>(field);
            }
            catch (Exception ex)
            {
                TraceError("GetIntValue", ex);
            }
            return value;
        }

        /// <summary>
        /// Aendert den Wert der angegebenen Spalte auf den uebergebenen Double-Wert
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Wert erfolgreich geaendert</returns>
        static public bool SetDoubleValue(this DataRow row, string field, double value)
        {
            bool result;
            try
            {
                row[field] = value;
                result = true;
            }
            catch (Exception ex)
            {
                TraceError("SetDoubleValue", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aendert den Wert der angegebenen Spalte auf den uebergebenen Integer-Wert
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Wert erfolgreich geaendert</returns>
        static public bool SetIntValue(this DataRow row, string field, int value)
        {
            bool result;
            try
            {
                row[field] = value;
                result = true;
            }
            catch (Exception ex)
            {
                TraceError("SetIntValue", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Aendert den Wert der angegebenen Spalte auf das uebergebene Datum
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Wert erfolgreich geaendert</returns>
        static public bool SetDateValue(this DataRow row, string field, DateTime value)
        {
            bool result;
            try
            {
                row[field] = value;
                result = true;
            }
            catch (Exception ex)
            {
                TraceError("SetDateValue", ex);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Aendert den Wert der angegebenen Spalte auf das uebergebene Datum
        /// </summary>
        /// <param name="row">Zeile</param>
        /// <param name="field">Name der Spalte</param>
        /// <param name="value">Neuer Wert</param>
        /// <returns>Wert erfolgreich geaendert</returns>
        static public bool SetStringValue(this DataRow row, string field, string value)
        {
            bool result;
            try
            {
                row[field] = value;
                result = true;
            }
            catch (Exception ex)
            {
                TraceError("SetStringValue", ex);
                result = false;
            }
            return result;
        }

        static void TraceError(string method, Exception ex)
        {
            Log.Write("DataRowExtensions", method, ex);
        }
    }
}
