using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Basics;
using System.Data.Common;

namespace Persistence
{
    /// <summary>
    /// Abstrakte Basisklasse, um eine Datenbank auf der 
    /// das Programm zugreift, zu verwalten
    /// </summary>
    internal abstract class Database
    {
        /// <summary>
        /// Oeffnet die Datenbank
        /// </summary>
        /// <param name="connectionString">Verbindungsparameter</param>
        /// <param name="error">Fehler-Object</param>
        /// <returns>Datenbank erfolgreich geöffnet</returns>
        public abstract bool Open(string connectionString, Error error);

        /// <summary>
        /// Schliesst die Datenbank
        /// </summary>
        /// <param name="error">Fehler-Object</param>
        /// <returns>Datenbank erfolgreich geschlossen</returns>
        public abstract bool Close(Error error);

        /// <summary>
        /// Liest aus der Datenbank Daten und füllt sie in die Datatable
        /// </summary>
        /// <param name="query">Beschreibung der zu lesenden Daten in SQL</param>
        /// <param name="dt">Zu füllende Datatable</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Datatable erfolgreich gefüllt</returns>
        public abstract bool LoadTable(string query, DataTable dt, Error error);

        /// <summary>
        /// Liest einen Datensatz aus der Datenbank und füllt den Inhalt in das übergebene Objekt
        /// </summary>
        /// <param name="query">Beschreibung des zu lesenden Datensatz in SQL</param>
        /// <param name="bo">Zu füllendes Objekt</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Objekt erfolgreich gefüllt</returns>
        public abstract bool LoadObject(string query, BaseObject bo, Error error);

        /// <summary>
        /// Liest Felder aus einen Datensatz aus der Datenbank und füllt den Inhalt das übergebene Objekt
        /// </summary>
        /// <param name="query">Beschreibung des zu lesenden Datensatz in SQL</param>
        /// <param name="bo">Zu füllendes Objekt</param>
        /// <param name="fields">Zu lesende Felder</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Objekt erfolgreich gefüllt</returns>
        public abstract bool LoadObject(string query, BaseObject bo, IEnumerable<string> fields, Error error);

        /// <summary>
        /// Liest Informationen aus der Datenbank und füllt den Inhalt in die übergenene Liste von Objekten
        /// </summary>
        /// <typeparam name="T">Typ des zu füllenden Objekts</typeparam>
        /// <param name="query">Beschreibung der zu lesenden Daten in SQL</param>
        /// <param name="bos">Zu füllende Liste mit Objekten</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Kein Fehler beim Füllen der Liste mit Objekten</returns>
        public abstract bool LoadObjects<T>(string query, BaseObjects bos, Error error) where T : BaseObject;

        /// <summary>
        /// Führt einen SQL-Befehl aus. Dabei werden Informationen aus dem übergebenen Objekt 
        /// in die Datenbank geschrieben oder ein Objekt gelöscht.
        /// </summary>
        /// <param name="query">Auszuführender SQL-Befehl</param>
        /// <param name="bo">Zu lesendes Objekt</param>
        /// <param name="fields">Zu lesende Felder als Enumeration</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>SQL-Befehl erfolgreich ausgeführt</returns>
        public abstract bool Execute(string query, BaseObject bo, IEnumerable<string> fields, Error error);

        /// <summary>
        /// Startet eine Transaktion in der Datenbank
        /// </summary>
        /// <param name="action">Beschreibung der Transaktion</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Transaktion erfolgreich gestartet</returns>
        public abstract bool StartTransaction(Error error);

        /// <summary>
        /// Stellt fest, ob zurzeit gerade eine Transaktion ausgefuehrt wird
        /// </summary>
        /// <returns>Transaktion wird gerade ausgefuehrt</returns>
        public abstract bool ExecutesTransaction();

        /// <summary>
        /// Beendet eine Transaktion
        /// </summary>
        /// <param name="action">Beschreibung der Transaktion</param>
        /// <param name="successful">Alle Operationen der Transaktion waren erfolgreich</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Transaktion erfolgreich beendet</returns>
        public abstract bool FinishTransaction(bool successful, Error error);

        /// <summary>
        /// Stellt fest, ob die Datenbank geoeffnet ist
        /// </summary>
        public bool IsOpen
        {
            get;
            set;
        }
    }
}
