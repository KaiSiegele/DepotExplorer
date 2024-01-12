using System.Windows.Input;
using Basics;
using UserInterface;
using System.Diagnostics;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl, Neuerfassung, Änderung und Ansicht von Depots
    /// </summary>
    public partial class DepotExplorerControl : BaseExplorerControl
    {
        public DepotExplorerControl()
        {
            InitializeComponent();
            InitCommandBindings();

            CtrlSelectDepot.Selected += OnDepotSelected;
        }

        #region Overrides von ExplorerControl
        protected override BaseObject GetSelectedObject()
        {
            return CtrlSelectDepot.SelectedDepot;
        }

        protected override BaseObject GetEditingObject()
        {
            return CtrlEditDepot.GetObject();
        }

        protected override EditingModus GetEditingModus()
        {
            return CtrlEditDepot.Modus;
        }
        #endregion

        #region Eventhandler
        private void ExecuteRemoveDepot(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveDepot();
        }

        private void CanExecuteRemoveDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RemoveObjectAllowed();
        }

        private void ExecuteOpenDepot(object sender, ExecutedRoutedEventArgs e)
        {
            EditDepot(true);
        }

        private void CanExecuteOpenDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void ExecuteInsertDepot(object sender, ExecutedRoutedEventArgs e)
        {
            InsertDepot();
        }

        private void CanExecuteInsertDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = InsertObjectAllowed();
        }

        private void ExecuteViewDepot(object sender, ExecutedRoutedEventArgs e)
        {
            EditDepot(false);
        }

        private void CanExecuteViewDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void OnDepotSelected(BaseObject baseObject)
        {
            if (OpenObjectAllowed())
            {
                EditDepot(false);
            }
        }
        #endregion

        #region Methoden fuer die Bearbeitung

        private void InsertDepot()
        {
            if (CheckNoEditings())
            {
                CtrlEditDepot.StartInsertingDepot();
            }
        }

        private void EditDepot(bool isModifying)
        {
            if (CheckNoEditings())
            {
                Depot depot = CtrlSelectDepot.SelectedDepot;
                Debug.Assert(depot != null);
                CtrlEditDepot.StartEditingDepot(depot, isModifying);
            }
        }

        private void RemoveDepot()
        {
            Depot depot = CtrlSelectDepot.SelectedDepot;
            Debug.Assert(depot != null);
            if (Prompts.AskUser(Properties.Resources.Title, Properties.Resources.DeleteDepot, depot))
            {
                var ddo = new DepotDataObject(depot);
                int id = depot.Id;
                if (ddo.Remove())
                    CtrlEditDepot.TryFinishViewingObject(id);
            }

        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Open);
            cb.Executed += ExecuteOpenDepot;
            cb.CanExecute += CanExecuteOpenDepot;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.New);
            cb.Executed += ExecuteInsertDepot;
            cb.CanExecute += CanExecuteInsertDepot;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Delete);
            cb.Executed += ExecuteRemoveDepot;
            cb.CanExecute += CanExecuteRemoveDepot;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ViewBaseObject);
            cb.Executed += ExecuteViewDepot;
            cb.CanExecute += CanExecuteViewDepot;
            CommandBindings.Add(cb);
        }

        /// <summary>
        /// Stellt vor der Bearbeitung sicher, dass kein Wertpapier und keine Bank bearbeitet wird
        /// </summary>
        /// <returns>true, wenn keines der Objekte bearbeitet wird, false sonst</returns>
        private bool CheckNoEditings()
        {
            bool result = false;
            string title = Properties.Resources.Title;

            if (UserEditings.CheckNoUserEditing(Constants.BankEditControlID, true, title, Properties.Resources.CannotWorkWithDepotWhileEditingBank))
            {
                if (UserEditings.CheckNoUserEditing(Constants.AktieEditControlID, true, title, Properties.Resources.CannotWorkWithDepotWhileEditingAktie))
                {
                    result = UserEditings.CheckNoUserEditing(Constants.FondEditControlID, true, title, Properties.Resources.CannotWorkWithDepotWhileEditingFond);
                }
            }
            return result;
        }
    }
}
