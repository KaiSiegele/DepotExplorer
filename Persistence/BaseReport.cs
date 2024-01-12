using System.Data;
using Tools;

namespace Persistence
{
    /// <summary>
    /// Abstrakte Basisklasse, um eine Tabelle fuer
    /// einen Report zu erstellen un mit Daten zu füllen
    /// </summary>
    public abstract class BaseReport
    {
        /// <summary>
        /// Füllt die Tabelle für den Report
        /// </summary>
        public abstract bool FillTable();

        /// <summary>
        /// Löscht die Tabelle des Reports
        /// </summary>
        public void ClearTable()
        {
            _dataTable.Clear();
        }

        public DataTable DataTable
        {
            get
            {
                return _dataTable;
            }
        }

        /// <summary>
        /// Lädt Daten aus der Datenbank in eine Tabelle
        /// </summary>
        /// <param name="description">Beschreibung der Abfrage</param>
        /// <param name="query">Abfrage</param>
        /// <param name="reportTable">Tabelle</param>
        /// <returns>Abfrage war erfolgreich</returns>
        protected bool LoadTable(string description, string query, DataTable reportTable)
        {
            ReportCommand command = new ReportCommand(description, query, reportTable);
            return CommandInterpreter.ExecuteLoadCommand(command);
        }

        /// <summary>
        /// Fuegt in die Daten-Tabelle eine neue Spalte zu
        /// </summary>
        /// <param name="column">Name der neuen Spalte</param>
        /// <param name="typename">Name des Typs</param>
        protected void AddColumn(string column, string typeName)
        {
            _dataTable.AddColumn(column, typeName);
        }

        protected DataTable _dataTable = new DataTable();
    }
}
