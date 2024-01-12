using System.Windows.Input;
using UserInterface;
using Basics;
using System.Diagnostics;
using VermoegensData;
using Tools;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl, Neuerfassung, Änderung und Ansicht von Fonds
    /// </summary>
    public partial class FondExplorerControl : BaseExplorerControl
    {
        public FondExplorerControl()
        {
            InitializeComponent();
            InitCommandBindings();

            CtrlSelectFond.Selected += OnFondSelected;
        }

        #region Overrides von ExplorerControl
        protected override BaseObject GetSelectedObject()
        {
            return CtrlSelectFond.SelectedFond;
        }

        protected override BaseObject GetEditingObject()
        {
            return CtrlEditFond.GetObject();
        }
        protected override EditingModus GetEditingModus()
        {
            return CtrlEditFond.Modus;
        }
        #endregion

        #region Eventhandler
        private void ExecuteRemoveFond(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveFond();
        }

        private void CanExecuteRemoveFond(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RemoveObjectAllowed();
        }

        private void ExecuteOpenFond(object sender, ExecutedRoutedEventArgs e)
        {
            EditFond(true);
        }

        private void CanExecuteOpenFond(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void ExecuteInsertFond(object sender, ExecutedRoutedEventArgs e)
        {
            InsertFond();
        }

        private void CanExecuteInsertFond(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = InsertObjectAllowed();
        }

        private void ExecuteViewFond(object sender, ExecutedRoutedEventArgs e)
        {
            EditFond(false);
        }

        private void CanExecuteViewFond(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }


        private void OnFondSelected(BaseObject baseObject)
        {
            if (OpenObjectAllowed())
            {
                EditFond(false);
            }
        }
        #endregion

        #region Methoden fuer die Bearbeitung
        private void InsertFond()
        {
            if (CheckNoUserEditings(true, false))
            {
                int anbieter = CtrlSelectFond.SelectedAnbieter;
                if (anbieter == BaseObjectConverter.All)
                {
                    anbieter = MasterData.DefaultAnbieterId;
                }
                CtrlEditFond.StartInsertingFond(anbieter);
            }
        }

        private void EditFond(bool isModifying)
        {
            if (CheckNoUserEditings(isModifying, false))
            {
                Fond fond = CtrlSelectFond.SelectedFond;
                Debug.Assert(fond != null);
                CtrlEditFond.StartUpdatingFond(fond, isModifying);
            }
        }

        private void RemoveFond()
        {
            if (CheckNoUserEditings(true, true))
            {
                Fond fond = CtrlSelectFond.SelectedFond;
                Debug.Assert(fond != null);
                var fdo = new FondDataObject(fond);
                if (!fdo.FindZuordnungen())
                {
                    if (Prompts.AskUser(Properties.Resources.Title, Properties.Resources.DeleteFonds, fond))
                    {
                        int id = fond.Id;
                        if (fdo.Remove())
                            CtrlEditFond.TryFinishViewingObject(id);
                    }
                }
                else
                {
                    var message = Properties.Resources.ResourceManager.GetMessageFromResource("CannotDeleteWertpapierMappedToDepot", fond);
                    Prompts.ShowInfo(Properties.Resources.Title, message);
                }
            }
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Delete);
            cb.Executed += ExecuteRemoveFond;
            cb.CanExecute += CanExecuteRemoveFond;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Open);
            cb.Executed += ExecuteOpenFond;
            cb.CanExecute += CanExecuteOpenFond;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.New);
            cb.Executed += ExecuteInsertFond;
            cb.CanExecute += CanExecuteInsertFond;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ViewBaseObject);
            cb.Executed += ExecuteViewFond;
            cb.CanExecute += CanExecuteViewFond;
            CommandBindings.Add(cb);
        }

        /// <summary>
        /// Stellt vor der Bearbeitung sicher, dass kein Jahr und kein Depot geöffnet ist
        /// </summary>
        /// <param name="isModifying">Zeigt an, dass der Fond verändert werden soll</param>
        /// <param name="remove">Fond soll gelöscht werden</param>
        /// <returns>true, wenn kein Jahr und kein Depot geöffnet ist, false sonst</returns>
        private bool CheckNoUserEditings(bool isModifying, bool remove)
        {
            bool result = false;
            string title = Properties.Resources.Title;

            string message = (remove ? Properties.Resources.CannotRemoveFondWhileWorkWithYear : Properties.Resources.CannotEditFondWhileWorkWithYear);
            if (UserEditings.CheckNoUserEditing(Constants.JahrEditControlID, !isModifying, title, message))
            {
                if (isModifying)
                {
                    message = (remove ? Properties.Resources.CannotRemoveFondWhileWorkWithDepot : Properties.Resources.CannotEditFondWhileWorkWithDepot);
                    result = UserEditings.CheckNoUserEditing(Constants.DepotEditControlID, false, title, message);
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
