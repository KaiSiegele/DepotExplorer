using Basics;
using System.Diagnostics;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl der Jahre und Änderung der Kurswerte
    /// </summary>
    public partial class JahrExplorerControl : BaseExplorerControl
    {
        public JahrExplorerControl()
        {
            InitializeComponent();
            InitCommandBindings();

            CtrlSelectJahr.Selected += OnJahrSelected;
        }

        #region Overrides von ExplorerControl
        protected override BaseObject GetSelectedObject()
        {
            return CtrlSelectJahr.SelectedJahr;
        }

        protected override BaseObject GetEditingObject()
        {
            return CtrlEditJahr.GetObject();
        }

        protected override EditingModus GetEditingModus()
        {
            return CtrlEditJahr.Modus;
        }
        #endregion

        #region Eventhandler

        private void ExecuteOpenJahr(object sender, ExecutedRoutedEventArgs e)
        {
            EditJahr(true);
        }

        private void CanExecuteViewJahr(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void ExecuteViewJahr(object sender, ExecutedRoutedEventArgs e)
        {
            EditJahr(false);
        }

        private void CanExecuteOpenJahr(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OpenObjectAllowed();
        }

        private void OnJahrSelected(BaseObject baseObject)
        {
            if (OpenObjectAllowed())
            {
                EditJahr(false);
            }
        }
        #endregion

        private void EditJahr(bool isModifying)
        {
            if (CheckNoUserEditings(isModifying))
            {
                Jahr jahr = CtrlSelectJahr.SelectedJahr;
                Debug.Assert(jahr != null);
                CtrlEditJahr.StartUpdatingJahr(jahr, isModifying);
            }
        }

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Open);
            cb.Executed += ExecuteOpenJahr;
            cb.CanExecute += CanExecuteOpenJahr;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ViewBaseObject);
            cb.Executed += ExecuteViewJahr;
            cb.CanExecute += CanExecuteViewJahr;
            CommandBindings.Add(cb);
        }

        /// <summary>
        /// Stellt vor der Bearbeitung sicher, dass kein Wertpapie bearbeitet wird
        /// <param name="isModifying">Zeigt an, dass das Jahr verändert werden soll</param>
        /// </summary>
        /// <returns>true, wenn keine Wertpapier bearbeitet wird, false sonst</returns>
        private bool CheckNoUserEditings(bool isModifying)
        {
            bool result = false;
            string title = Properties.Resources.Title;
            bool mustEditingWertpapier = (isModifying == false);

            if (UserEditings.CheckNoUserEditing(Constants.AktieEditControlID, mustEditingWertpapier, Properties.Resources.Title, Properties.Resources.CannotWorkWithYearWhileEditingAktie))
            {
                result = UserEditings.CheckNoUserEditing(Constants.FondEditControlID, mustEditingWertpapier, Properties.Resources.Title, Properties.Resources.CannotWorkWithYearWhileEditingFond);
            }
            return result;
        }
    }
}
