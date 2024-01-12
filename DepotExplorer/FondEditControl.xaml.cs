using Basics;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht von Fonds
    /// </summary>
    public partial class FondEditControl : BaseObjectEditControl
    {
        public FondEditControl()
        {
            InitializeComponent();

            InitComboBoxes();
            _fondViewModel = new FondViewModel();

            GBData.DataContext = _fondViewModel;
            GBClassifikation.DataContext = _fondViewModel;

            _fondViewModel.Update(BaseObject.CreateDefault<Fond>(), false);

            InitCommandBindings();
        }

        internal void StartInsertingFond(int anbieter)
        {
            BaseObject.CheckId(anbieter);
            Fond fond = Fond.CreateFond(anbieter);
            if (CheckNewObject(Properties.Resources.Title, Properties.Resources.Fond, fond))
            {
                StartEditingObject(fond, EditingModus.Inserting);
            }
        }

        internal void StartUpdatingFond(Fond fond, bool isModifying)
        {
            BaseObject fond2Edit = fond.Copy();
            Kurse kurse = new Kurse();
            var fdo = new FondDataObject(fond2Edit, kurse);

            if (fdo.Load())
            {
                CtrlEditKurse.PrepareEditingKurse(kurse);
                StartEditingObject(fond2Edit, isModifying ? EditingModus.Updating : EditingModus.Viewing);
            }
        }

        #region Overrides von BaseEditControl
        public override BaseObject GetObject()
        {
            return _fondViewModel.BaseObject;
        }

        protected override bool HasObjectChanged()
        {
            return _fondViewModel.Changed || CtrlEditKurse.Changed;
        }

        protected override bool IsObjectValid()
        {
            return _fondViewModel.Valid && CtrlEditKurse.Valid;
        }

        protected override void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
            UserEditings.InsertUserEditing(Constants.FondEditControlID, this, modus, bo);
            _fondViewModel.Update(bo, modus.IsModifying());
            CtrlEditKurse.StartEditingContent(bo.Id, !modus.IsModifying());
        }

        protected override void UpdateAfterEditing()
        {
            UserEditings.RemoveUserEditing(Constants.FondEditControlID);
            _fondViewModel.Update(BaseObject.CreateDefault<Fond>(), false);
            CtrlEditKurse.FinishEditingContent(true);
            TabControlFond.SelectedIndex = 0;
        }

        protected override void UpdateAfterSaving()
        {
            _fondViewModel.Reset();
            CtrlEditKurse.FinishEditingContent(false);
        }
        #endregion

        #region Eventhandler
        private void ExecuteSaveFond(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingFond(false);
        }

        private void ExecuteSaveFondAndClose(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingFond(true);
        }

        private void CanExecuteSaveFond(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveObjectAllowed();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            HandleCancelEditing(Properties.Resources.Title, Properties.Resources.FondHasChanged);
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editing;
        }
        #endregion

        private void InitComboBoxes()
        {
            UserInterface.Tools.FillCombox<FondTyp>(CbTyp, true);

            CbAnbieter.ItemsSource = MasterData.Anbieter.ObjectNames;
            CbTheme.ItemsSource = BaseObjectConverter.GetObjectNamesWithNotSpecified(MasterData.Themen.ObjectNames);
            CbRegion.ItemsSource = BaseObjectConverter.GetObjectNamesWithNotSpecified(MasterData.Regionen.ObjectNames);
        }

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save);
            cb.Executed += ExecuteSaveFond;
            cb.CanExecute += CanExecuteSaveFond;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.SaveAndClose);
            cb.Executed += ExecuteSaveFondAndClose;
            cb.CanExecute += CanExecuteSaveFond;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Stop);
            cb.Executed += ExecuteCancelEditing;
            cb.CanExecute += CanExecuteCancelEditing;
            CommandBindings.Add(cb);
        }

        private void FinishEditingFond(bool close)
        {
            if (CtrlEditKurse.CheckKurse())
            {
                BaseObject fond = _fondViewModel.BaseObject;
                Kurse kurse = CtrlEditKurse.Kurse;
                var fdo = new FondDataObject(fond, kurse);

                SaveObject(fdo, close);
            }
        }

        private readonly FondViewModel _fondViewModel = null;
    }
}
