using Basics;
using System.Diagnostics;
using System.Windows.Input;
using UserInterface;
using VermoegensData;
using Tools;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl, Neuerfassung, Änderung und Ansicht von Aktien
    /// </summary>
    public partial class AktieExplorerControl : BaseExplorerControl
    {
        public AktieExplorerControl()
        {
            InitializeComponent();
            InitCommandBindings();

            CtrlSelectAktie.Selected += OnAktieSelected;
        }

        #region Overrides von ExplorerControl
        protected override BaseObject GetSelectedObject()
        {
            return CtrlSelectAktie.SelectedAktie;
        }

        protected override BaseObject GetEditingObject()
        {
            return CtrlEditAktie.GetObject();
        }
        protected override EditingModus GetEditingModus()
        {
            return CtrlEditAktie.Modus;
        }
        #endregion

        #region Eventhandler
        private void ExecuteRemoveAktie(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveAktie();
        }

        private void CanExecuteRemoveAktie(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RemoveObjectAllowed();
        }

        private void ExecuteOpenAktie(object sender, ExecutedRoutedEventArgs e)
        {
            EditAktie(true);
        }

        private void CanExecuteOpenAktie(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void ExecuteInsertAktie(object sender, ExecutedRoutedEventArgs e)
        {
            InsertAktie();
        }

        private void ExecuteViewAktie(object sender, ExecutedRoutedEventArgs e)
        {
            EditAktie(false);
        }

        private void CanExecuteViewAktie(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void CanExecuteInsertAktie(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = InsertObjectAllowed();
        }

        private void OnAktieSelected(BaseObject baseObject)
        {
            if (OpenObjectAllowed())
            {
                EditAktie(false);
            }
        }
        #endregion

        #region Methoden fuer die Bearbeitung
        private void InsertAktie()
        {
            if (CheckNoUserEditings(true, false))
            {
                int land = CtrlSelectAktie.SelectedLand;
                if (land == BaseObjectConverter.All)
                {
                    land = MasterData.DefaultLandId;
                }
                CtrlEditAktie.StartInsertingAktie(land);
            }
        }

        private void EditAktie(bool isModifying)
        {
            if (CheckNoUserEditings(isModifying, false))
            {
                Aktie aktie = CtrlSelectAktie.SelectedAktie;
                Debug.Assert(aktie != null);
                CtrlEditAktie.StartEditingAktie(aktie, isModifying);
            }
        }

        private void RemoveAktie()
        {
            if (CheckNoUserEditings(true, true))
            {
                Aktie aktie = CtrlSelectAktie.SelectedAktie;
                Debug.Assert(aktie != null);
                var ado = new AktieDataObject(aktie);
                if (!ado.FindZuordnungen())
                {
                    if (Prompts.AskUser(Properties.Resources.Title, Properties.Resources.DeleteAktie, aktie))
                    {
                        int id = aktie.Id;
                        if (ado.Remove())
                            CtrlEditAktie.TryFinishViewingObject(id);
                    }
                }
                else
                {
                    var message = Properties.Resources.ResourceManager.GetMessageFromResource("CannotDeleteWertpapierMappedToDepot", aktie);
                    Prompts.ShowInfo(Properties.Resources.Title, message);
                }
            }
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Delete);
            cb.Executed += ExecuteRemoveAktie;
            cb.CanExecute += CanExecuteRemoveAktie;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Open);
            cb.Executed += ExecuteOpenAktie;
            cb.CanExecute += CanExecuteOpenAktie;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.New);
            cb.Executed += ExecuteInsertAktie;
            cb.CanExecute += CanExecuteInsertAktie;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ViewBaseObject);
            cb.Executed += ExecuteViewAktie;
            cb.CanExecute += CanExecuteViewAktie;
            CommandBindings.Add(cb);
        }

        /// <summary>
        /// Stellt vor der Bearbeitung sicher, dass kein Jahr und kein Depot geöffnet ist
        /// </summary>
        /// <param name="isModifying">Zeigt an, dass die Aktie verändert werden soll</param>
        /// <param name="remove">Aktie soll gelöscht werden</param>
        /// <returns>true, wenn kein Jahr und kein Depot geöffnet ist, false sonst</returns>
        private bool CheckNoUserEditings(bool isModifying, bool remove)
        {
            bool result = false;
            string title = Properties.Resources.Title;
            string message = (remove ? Properties.Resources.CannotRemoveAktieWhileWorkWithYear : Properties.Resources.CannotEditAktieWhileWorkWithYear);
            if (UserEditings.CheckNoUserEditing(Constants.JahrEditControlID, !isModifying, title, message))
            {
                if (isModifying)
                {
                    message = (remove ? Properties.Resources.CannotRemoveAktieWhileWorkWithDepot : Properties.Resources.CannotEditAktieWhileWorkWithDepot);
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
