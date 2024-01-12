using System;
using System.Collections.Generic;
using System.Windows;
using UserInterface;
using Persistence;
using VermoegensData;
using Tools;

namespace DepotExplorer
{
    /// <summary>
    /// Hauptfenster der Anwendung
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CommandInterpreter.Executed += OnCommandExecuted;

            Starter.LoadCaches();
        }

        #region Eventhandler
        private void OnCommandExecuted(object sender, CommandEventArgs args)
        {
            if (args.Result == false)
            {
                Prompts.ShowError(Properties.Resources.Title, args.Error.Description);
            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string objectsEditing;
            if (CheckForEditings(out objectsEditing))
            {
                string foundEditings = string.Format(Properties.Resources.DataAreChanged, objectsEditing);
                e.Cancel = !Prompts.AskUser(Properties.Resources.Title, foundEditings + Properties.Resources.TerminateProgramm);
            }
        }
        #endregion

        /// <summary>
        /// Stellt fest, ob in einem Editor noch Objekte geändert wurden
        /// </summary>
        /// <param name="objectsEditing">Liste von Objekten, die gerade geändert werden</param>
        /// <returns>true, wenn gerade Objekte geändert werden, false sonst</returns>
        private bool CheckForEditings(out string objectsEditing)
        {
            List<Tuple<int, string>> lstControls = new List<Tuple<int, string>>();
            Action<int, string> addEntry = (controlId, typename) =>
            {
                lstControls.Add(new Tuple<int, string>(controlId, typename));
            };
            addEntry(Constants.BankEditControlID, Properties.Resources.Bank);
            addEntry(Constants.DepotEditControlID, Properties.Resources.Depot);
            addEntry(Constants.FondEditControlID, Properties.Resources.Fond);
            addEntry(Constants.AktieEditControlID, Properties.Resources.Aktie);
            addEntry(Constants.JahrEditControlID, Properties.Resources.Year);

            List<string> lstObjectsEditing = new List<string>();

            foreach (var control in lstControls)
            {
                if (UserEditings.HasUserEditing(control.Item1, true))
                {
                    lstObjectsEditing.Add(control.Item2);
                }
            }

            if (lstObjectsEditing.Count > 0)
            {
                objectsEditing = Misc.GetNamesAsString(lstObjectsEditing);
                return true;
            }
            else
            {
                objectsEditing = string.Empty;
                return false;
            }
        }
    }
}
