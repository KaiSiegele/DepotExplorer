using System.Diagnostics;
using System.Data;
using Persistence.Properties;
using Basics;

namespace Persistence
{
    /// <summary>
    /// Befehl zum Befuellen einer Datatable anhand einer Abfrage
    /// Der Inhalt der Datatable ist der Report
    /// </summary>
    public class ReportCommand : BaseCommand
    {
        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="reportDescription">Beschreibung des Reports</param>
        /// <param name="query">Abfrage mit der der Report gefuellt werden soll</param>
        /// <param name="dataTable">Datatable um die Daten aufzunehmen</param>
        public ReportCommand(string reportDescription, string query, DataTable dataTable)
        {
            CheckParams(reportDescription, query, dataTable);
            _reportDescription = reportDescription;
            _query = query;
            _dataTable = dataTable;
        }

        #region Ueberschreibungen der Basisklasse
        public override string GetDescription(CommandTyp commandTyp)
        {
            string action;
            switch (commandTyp)
            {
                case CommandTyp.Load:
                    action = Resource.Load;
                    break;
                default:
                    action = string.Empty;
                    break;
            }
            return action + " " + _reportDescription;
        }

        public override bool Execute(CommandTyp commandTyp, Error error)
        {
            bool result;
            switch (commandTyp)
            {
                case CommandTyp.Load:
                    result = DatabaseHandle.LoadTable(_query, _dataTable, error);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
        #endregion

        [Conditional("DEBUG")]
        private static void CheckParams(string reportDescription, string query, DataTable dataTable)
        {
            Debug.Assert(!(string.IsNullOrEmpty(reportDescription)), "Beschreibung nicht angegeben");
            Debug.Assert(!(string.IsNullOrEmpty(query)), "Abfrage nicht angegeben");
            Debug.Assert(!(dataTable == null), "Datatable nicht angegeben");
        }

        private readonly DataTable _dataTable;
        private readonly string _query;
        private readonly string _reportDescription;
    }
}