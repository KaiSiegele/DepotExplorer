using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Basics;
using Tools;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl, Neuerfassung, Änderung und Ansicht von Banken
    /// </summary>
    public partial class BankExplorerControl : BaseExplorerControl
    {
        public BankExplorerControl()
        {
            InitializeComponent();
            InitCommandBindings();

            CtrlSelectBank.Selected += OnBankSelected;
        }

        #region Overrides von ExplorerControl
        protected override BaseObject GetSelectedObject()
        {
            return CtrlSelectBank.SelectedBank;
        }

        protected override BaseObject GetEditingObject()
        {
            return CtrlEditBank.GetObject();
        }

        protected override EditingModus GetEditingModus()
        {
            return CtrlEditBank.Modus;
        }
        #endregion

        #region Eventhandler
        private void ExecuteRemoveBank(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveBank();
        }

        private void CanExecuteRemoveBank(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RemoveObjectAllowed();
        }

        private void ExecuteOpenBank(object sender, ExecutedRoutedEventArgs e)
        {
            EditBank(true);
        }

        private void CanExecuteOpenBank(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void ExecuteInsertBank(object sender, ExecutedRoutedEventArgs e)
        {
            InsertBank();
        }

        private void CanExecuteInsertBank(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = InsertObjectAllowed();
        }
        private void ExecuteViewBank(object sender, ExecutedRoutedEventArgs e)
        {
            EditBank(false);
        }

        private void CanExecuteViewBank(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void OnBankSelected(BaseObject baseObject)
        {
            if (OpenObjectAllowed())
            {
                EditBank(false);
            }
        }
        #endregion

        #region Methoden fuer die Bearbeitung
        private void InsertBank()
        {
            if (CheckNoEditings(true, false))
            {
                CtrlEditBank.StartInsertingBank();
            }
        }

        private void EditBank(bool isModifying)
        {
            if (CheckNoEditings(isModifying, false))
            {
                Bank bank = CtrlSelectBank.SelectedBank;
                Debug.Assert(bank != null);
                CtrlEditBank.StarEditingBank(bank, isModifying);
            }
        }

        private void RemoveBank()
        {
            if (CheckNoEditings(true, true))
            {
                Bank bank = CtrlSelectBank.SelectedBank;
                Debug.Assert(bank != null);
                int id = bank.Id;
                var depots = CachedData.Depots.GetDepotsUsingBank(id);
                if (!depots.Any())
                {
                    if (Prompts.AskUser(Properties.Resources.Title, Properties.Resources.DeleteBank, bank))
                    {
                        var bdo = new BankDataObject(bank);
                        if (bdo.Remove())
                            CtrlEditBank.TryFinishViewingObject(id);
                    }
                }
                else
                {
                    List<string> depotNames = (from d in depots select d.Name).ToList();
                    var message = Properties.Resources.ResourceManager.GetMessageFromResource("CannotDeleteBankUsedByDepots", bank, Misc.GetNamesAsString(depotNames));
                    Prompts.ShowInfo(Properties.Resources.Title, message);
                }
            }
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Delete);
            cb.Executed += ExecuteRemoveBank;
            cb.CanExecute += CanExecuteRemoveBank;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Open);
            cb.Executed += ExecuteOpenBank;
            cb.CanExecute += CanExecuteOpenBank;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.New);
            cb.Executed += ExecuteInsertBank;
            cb.CanExecute += CanExecuteInsertBank;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ViewBaseObject);
            cb.Executed += ExecuteViewBank;
            cb.CanExecute += CanExecuteViewBank;
            CommandBindings.Add(cb);
        }

        /// <summary>
        /// Stellt vor der Bearbeitung sicher, dass kein Depot geöffnet ist
        /// </summary>
        /// <param name="isModifying">Zeigt an, dass die Bank verändert werden soll</param>
        /// <param name="remove">Bank soll gelöscht werden</param>
        /// <returns>true, wenn kein Depot geöffnet ist, false sonst</returns>
        private bool CheckNoEditings(bool isModifying, bool remove)
        {
            if (isModifying)
            {
                string message = (remove ? Properties.Resources.CannotRemoveBankWhileWorkWithDepot : Properties.Resources.CannotEditBankWhileWorkWithDepot);
                return UserEditings.CheckNoUserEditing(Constants.DepotEditControlID, false, Properties.Resources.Title, message);
            }
            else
            {
                return true;
            }
        }
    }
}