using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using Basics;
using Tools;
using Persistence.Properties;

namespace Persistence
{
    /// <summary>
    /// Enumeration fuer Datenbanken, die vom
    /// Programm angesprochen werden können
    /// </summary>
    public enum DBType
    {
        Unknown = 0,
        SQLCe,
        SQLServer,
        OLEDb
    }

    /// <summary>
    /// Klasse, um die Datenbank auf den das Programm
    /// zugreift zu verwalten
    /// </summary>
    public static class DatabaseHandle
    {
        /// <summary>
        /// Erzeugt eines Databank mit dem uebergebenen Typ
        /// </summary>
        /// <param name="dbType">Type der Datenbank</param>
        /// <param name="error">Fehler-Object</param>
        /// <returns>Erzeugung war erfolgreich</returns>
        public static bool CreateDatabase(DBType dbType, Error error)
        {
            bool result = false;
            switch (dbType)
            {
                case DBType.SQLCe:
                    database = new SQLCeDatabase();
                    result = true;
                    break;
                case DBType.SQLServer:
                    database = new SQLServerDatabase();
                    result = true;
                    break;
                default:
                    TraceError("CreateDatabase", "Datenbanktyp " + dbType.ToString() + " wird nicht unterstützt", error);
                    break;
            }
            if (result)
            {
                TraceInfo("CreateDatabase", "Datenbank vom Typ " + dbType.ToString() + " erfolgreich erstellt");
            }

            return result;
        }

        /// <summary>
        /// Oeffnet die zuvor erzeugte Datenbank
        /// </summary>
        /// <param name="connectionString">Verbindungsparameter</param>
        /// <param name="error">Fehler-Object</param>
        /// <returns>Datenbank erfolgreich geöffnet</returns>
        public static bool OpenDatabase(string connectionString, Error error)
        {
            bool result = false;
            if (database != null)
            {
                if (!database.IsOpen)
                {
                    if (database.Open(connectionString, error))
                    {
                        result = true;
                        TraceInfo("OpenDatabase", "Datenbank " + connectionString + " erfolgreich geöffnet");
                    }
                }
                else
                {
                    TraceError("OpenDatabase", "Datenbank ist bereits geöffnet", error);
                }
            }
            else
            {
                TraceError("OpenDatabase", "Datenbank noch nicht erzeugt", error);
            }

            return result;
        }

        /// <summary>
        /// Schliesst die Datenbank
        /// </summary>
        /// <param name="error">Fehler-Object</param>
        /// <returns>Datenbank erfolgreich geschlossen</returns>
        public static bool CloseDatabase(Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("CloseDatabase", error))
            {
                result = database.Close(error);
                if (result)
                    TraceInfo("CloseDatabase", "Datenbank erfolgreich geschlossen");
                else
                    TraceError("CloseDatabase", "Fehler beim Schließen der Datenbank", error);
            }
            return result;
        }

        /// <summary>
        /// Liest aus der Datenbank Daten und füllt sie in die Datatable
        /// </summary>
        /// <param name="query">Beschreibung der zu lesenden Daten in SQL</param>
        /// <param name="dt">Zu füllende Datatable</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Datatable erfolgreich gefüllt</returns>
        public static bool LoadTable(string query, DataTable dt, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("LoadTable", error))
            {
                result = database.LoadTable(query, dt, error);
            }
            TraceResult("LoadTable", query, result);
            return result;
        }

        /// <summary>
        /// Liest einen Datensatz aus der Datenbank und füllt den Inhalt das übergebene Objekt
        /// </summary>
        /// <param name="query">Beschreibung des zu lesenden Datensatz in SQL</param>
        /// <param name="bo">Zu füllendes Objekt</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Objekt erfolgreich gefüllt</returns>
        public static bool LoadObject(string query, BaseObject bo, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("LoadObject", error))
            {
                result = database.LoadObject(query, bo, error);
            }
            TraceResult("LoadObject", query, result);
            return result;
        }

        /// <summary>
        /// Liest Felder aus einen Datensatz aus der Datenbank und füllt den Inhalt das übergebene Objekt
        /// </summary>
        /// <param name="query">Beschreibung des zu lesenden Datensatz in SQL</param>
        /// <param name="bo">Zu füllendes Objekt</param>
        /// <param name="fields">Zu lesende Felder</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Objekt erfolgreich gefüllt</returns>
        public static bool LoadObject(string query, BaseObject bo, IEnumerable<string> fields, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("LoadObject", error))
            {
                result = database.LoadObject(query, bo, fields, error);
            }
            TraceResult("LoadObject", query, result);
            return result;
        }

        /// <summary>
        /// Liest Informationen aus der Datenbank und füllt den Inhalt in die übergenene Liste von Objekten
        /// </summary>
        /// <typeparam name="T">Typ des zu füllenden Objekts</typeparam>
        /// <param name="query">Beschreibung der zu lesenden Daten in SQL</param>
        /// <param name="bos">Zu füllende Liste mit Objekten</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Kein Fehler beim Füllen der Liste mit Objekten</returns>
        public static bool LoadObjects<T>(string query, BaseObjects bos, Error error) where T : BaseObject
        {
            bool result = false;
            if (CheckIfDatabaseOpen("LoadObjects<T>", error))
            {
                result = database.LoadObjects<T>(query, bos, error);
            }
            TraceResult("LoadObjects<T>", query, result);
            return result;
        }

        /// <summary>
        /// Führt einen SQL-Befehl aus. Dabei werden Informationen aus dem übergebenen Objekt 
        /// in die Datenbank geschrieben oder ein Objekt gelöscht.
        /// </summary>
        /// <param name="query">Auszuführender SQL-Befehl</param>
        /// <param name="bo">Zu lesendes Objekt</param>
        /// <param name="fields">Zu lesende Felder</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>SQL-Befehl erfolgreich ausgeführt</returns>
        public static bool Execute(string query, BaseObject bo, string fields, Error error)
        {
            return Execute(query, bo, fields.Split(','), error);
        }

        /// <summary>
        /// Führt einen SQL-Befehl aus. Dabei werden Informationen aus dem übergebenen Objekt 
        /// in die Datenbank geschrieben oder ein Objekt gelöscht.
        /// </summary>
        /// <param name="query">Auszuführender SQL-Befehl</param>
        /// <param name="bo">Zu lesendes Objekt</param>
        /// <param name="fields">Zu lesende Felder als Enumeration</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>SQL-Befehl erfolgreich ausgeführt</returns>
        public static bool Execute(string query, BaseObject bo, IEnumerable<string> fields, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("Execute", error))
            {
                result = database.Execute(query, bo, fields, error);
            }
            TraceResult("Execute", query, result);
            return result;
        }

        /// <summary>
        /// Startet eine Transaktion in der Datenbank
        /// </summary>
        /// <param name="action">Beschreibung der Transaktion</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Transaktion erfolgreich gestartet</returns>
        public static bool StartTransaction(string action, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("StartTransaction", error))
            {
                if (!database.ExecutesTransaction())
                {
                    TraceVerbose("StartTransaction", action);
                    result = database.StartTransaction(error);
                }
                else
                {
                    TraceError("StartTransaction", "Transaktion wird gerade ausgeführt", error);
                }
            }
            return result;
        }

        /// <summary>
        /// Beendet eine Transaktion
        /// </summary>
        /// <param name="action">Beschreibung der Transaktion</param>
        /// <param name="successful">Alle Operationen der Transaktion waren erfolgreich</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Transaktion erfolgreich beendet</returns>
        public static bool FinishTransaction(string action, bool successful, Error error)
        {
            bool result = false;
            if (CheckIfDatabaseOpen("FinishTransaction", error))
            {
                if (database.ExecutesTransaction())
                {
                    TraceVerbose("FinishTransaction", action + " " + (successful ? Resource.Successful : Resource.Failed));
                    result = database.FinishTransaction(successful, error);
                }
                else
                {
                    TraceError("FinishTransaction", "Es wird gerade keine Transaktion ausgeführt", error);
                }
            }
            return result;
        }

        private static bool CheckIfDatabaseOpen(string method, Error error)
        {
            if (database != null)
            {
                if (!database.IsOpen)
                {
                    TraceError(method, "Datenbank ist nicht geöffnet", error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                TraceError(method, "Datenbank noch nicht erzeugt", error);
                return false;
            }
        }

        #region Writing into trace
        private static void TraceResult(string method, string query, bool result)
        {
            Log.Write(TraceLevel.Verbose, ownClassName, method, "Abfrage \"{0}\" {1}", query, result ? Resource.Successful : Resource.Failed);
        }

        private static void TraceError(string method, string message, Error error)
        {
            DatabaseTools.HandleError(ownClassName, method, message, error);
        }

        private static void TraceInfo(string method, string message)
        {
            Log.Write(TraceLevel.Info, ownClassName, method, message);
        }

        private static void TraceVerbose(string method, string message)
        {
            Log.Write(TraceLevel.Verbose, ownClassName, method, message);
        }

        private static readonly string ownClassName = "DatabaseHandle";
        #endregion

        private static Database database = null;
    }
}
