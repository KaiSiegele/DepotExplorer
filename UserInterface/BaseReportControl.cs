using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für ein UserControl zum Erstellen von Reports
    /// Jedes UserControl hat im oberen Teil eine Auswahl für die
    /// Kriterien des Reports, im unteren Teil wird der Report
    /// angezeigt.
    /// </summary>
    public class BaseReportControl : UserControl
    {
        protected BaseReportControl()
        {
            ShowReport = false;
        }

        /// <summary>
        /// Methode die überschrieben werden muss, wenn im unteren
        /// Teil der Report gelöscht werden soll
        /// </summary>
        protected virtual void ClearReport()
        {

        }

        /// <summary>
        /// Methode die überschrieben werden muss, wenn im unteren
        /// Teil der Report angezeigt werden soll
        /// </summary>
        protected virtual bool LoadReport()
        {
            return false;
        }

        /// <summary>
        /// Methode die anzeigt, ob der Report angezeigt werden
        /// soll oder gelöscht werden muss, weil sich die Auswahl
        /// geändert hat
        /// </summary>
        /// <param name="showReport"></param>
        protected void UpdateReportMode(bool showReport)
        {
            if (showReport)
            {
                Debug.Assert(ShowReport == false);
                if (LoadReport())
                {
                    ShowReport = true;
                }
            }
            else
            {
                if (ShowReport)
                {
                    ClearReport();
                    ShowReport = false;
                }
            }
        }
        private bool ShowReport { get; set; }
    }
}
