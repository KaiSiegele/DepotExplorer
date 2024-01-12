using Basics;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht von Depots
    /// </summary>
    public partial class DepotEditControl : BaseObjectEditControl
    {
        public DepotEditControl()
        {
            InitializeComponent();

            _depotViewModel = new DepotViewModel();
            GBData.DataContext = _depotViewModel;
            GBName.DataContext = _depotViewModel;

            _zuordnungViewModels = new ZuordnungViewModels(_zuordnungen);
            ListBoxFonds.ItemsSource = _zuordnungViewModels;
            _zuordnungenChanged = false;

            ClearControl();
            InitCommandBindings();
        }

        internal void StartInsertingDepot()
        {
            Depot depot = Depot.CreateDepot();
            if (CheckNewObject(Properties.Resources.Title, Properties.Resources.Depot, depot))
            {
                StartEditingObject(depot, EditingModus.Inserting);
            }
        }

        internal void StartEditingDepot(Depot depot, bool isModifying)
        {
            BaseObject depot2Edit = depot.Copy();
            Bestaende bestaende = new Bestaende();
            Zuordnungen zuordnungen = new Zuordnungen();
            var ddo = new DepotDataObject(depot2Edit, bestaende, zuordnungen);

            if (ddo.Load())
            {
                CtrlEditBestaende.PrepareEditingBestaende(bestaende, zuordnungen.WertpapierIds);
                _zuordnungen.InsertObjects(zuordnungen);
                StartEditingObject(depot2Edit, isModifying ? EditingModus.Updating : EditingModus.Viewing);
            }
        }

        #region Overrides von BaseEditControl
        public override BaseObject GetObject()
        {
            return _depotViewModel.BaseObject;
        }

        protected override bool HasObjectChanged()
        {
            return _depotViewModel.Changed || _zuordnungenChanged || CtrlEditBestaende.Changed;
        }

        protected override bool IsObjectValid()
        {
            return _depotViewModel.Valid && CtrlEditBestaende.Valid;
        }

        protected override void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
            UserEditings.InsertUserEditing(Constants.DepotEditControlID, this, modus, bo);
            CbBank.ItemsSource = CachedData.Banken.ObjectNames;
            _depotViewModel.Update(bo, modus.IsModifying());
            _zuordnungenChanged = false;
            CtrlEditBestaende.StartEditingContent(bo.Id, !modus.IsModifying());
        }

        protected override void UpdateAfterEditing()
        {
            UserEditings.RemoveUserEditing(Constants.DepotEditControlID);
            CtrlEditBestaende.FinishEditingContent(true);
            ClearControl();
        }

        protected override void UpdateAfterSaving()
        {
            _depotViewModel.Reset();
            _zuordnungenChanged = false;
            CtrlEditBestaende.FinishEditingContent(false);
        }
        #endregion


        #region Eventhandler
        private void ExecuteSaveDepot(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingDepot(false);
        }
        private void ExecuteSaveDepotAndClose(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingDepot(true);
        }

        private void CanExecuteSaveDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveObjectAllowed();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            HandleCancelEditing(Properties.Resources.Title, Properties.Resources.DepotHasChanged);
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editing;
        }

        private void ExecuteManageZuordnungen(object sender, ExecutedRoutedEventArgs e)
        {
            ManageZuordnungen();
        }

        private void CanExecuteManageZuordnungen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Modifying;
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save);
            cb.Executed += ExecuteSaveDepot;
            cb.CanExecute += CanExecuteSaveDepot;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.SaveAndClose);
            cb.Executed += ExecuteSaveDepotAndClose;
            cb.CanExecute += CanExecuteSaveDepot;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Stop);
            cb.Executed += ExecuteCancelEditing;
            cb.CanExecute += CanExecuteCancelEditing;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ManageZuordnungen);
            cb.Executed += ExecuteManageZuordnungen;
            cb.CanExecute += CanExecuteManageZuordnungen;
            CommandBindings.Add(cb);
        }

        private void FinishEditingDepot(bool close)
        {
            if (CtrlEditBestaende.CheckBestaende())
            {
                BaseObject depot = _depotViewModel.BaseObject;
                Bestaende bestaende = CtrlEditBestaende.Bestaende;
                var ddo = new DepotDataObject(depot, bestaende, _zuordnungen);

                SaveObject(ddo, close);
            }
        }

        private void ManageZuordnungen()
        {
            ZuordnungsDialog dlg = new ZuordnungsDialog(_depotViewModel.BaseObject, _zuordnungen, CtrlEditBestaende.Bestaende);
            bool? result = dlg.ShowDialog();
            if (result.HasValue)
            {
                if (result == true)
                {
                    _zuordnungen.Clear();
                    _zuordnungen.InsertObjects(dlg.Zuordnungen);
                    CtrlEditBestaende.UpdateSelectableWertpapiere(_zuordnungen.WertpapierIds);
                    _zuordnungenChanged = true;
                }
            }
        }

        private void ClearControl()
        {
            _depotViewModel.Update(BaseObject.CreateDefault<Depot>(), false);
            CbBank.ItemsSource = null;
            _zuordnungen.ClearObjects();
            _zuordnungenChanged = false;
            TabControlDepot.SelectedIndex = 0;
        }

        private readonly DepotViewModel _depotViewModel = null;
        private readonly ZuordnungViewModels _zuordnungViewModels = null;
        private Zuordnungen _zuordnungen = new Zuordnungen();
        private bool _zuordnungenChanged;
    }
}
