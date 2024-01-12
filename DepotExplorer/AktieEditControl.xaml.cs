using Basics;
using System.Collections.Generic;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    ///  Control für die Neuerfassung, Änderung und Ansicht von Aktien
    /// </summary>
    public partial class AktieEditControl : BaseObjectEditControl
    {
        public AktieEditControl()
        {
            InitializeComponent();
            CbLand.ItemsSource = MasterData.Laender.ObjectNames;

            _aktieViewModel = new AktieViewModel();

            GBData.DataContext = _aktieViewModel;
            GBDescriptions.DataContext = _aktieViewModel;

            _aktieViewModel.Update(BaseObject.CreateDefault<Aktie>(), false); 
            
            InitCommandBindings();
        }

        internal void StartInsertingAktie(int land)
        {
            BaseObject.CheckId(land);
            Aktie aktie = Aktie.CreateAktie(land);
            if (CheckNewObject(Properties.Resources.Title, Properties.Resources.Aktie, aktie))
            {
                StartEditingObject(aktie, EditingModus.Inserting);
            }
        }

        internal void StartEditingAktie(Aktie aktie, bool isModifying)
        {
            BaseObject aktie2Edit = aktie.Copy();
            Kurse kurse = new Kurse();
            var ado = new AktieDataObject(aktie2Edit, kurse);

            if (ado.Load())
            {
                CtrlEditKurse.PrepareEditingKurse(kurse);
                StartEditingObject(aktie2Edit, isModifying ? EditingModus.Updating : EditingModus.Viewing);
            }
        }

        #region Overrides von BaseEditControl
        public override BaseObject GetObject()
        {
            return _aktieViewModel.BaseObject;
        }

        protected override bool HasObjectChanged()
        {
            return _aktieViewModel.Changed || CtrlEditKurse.Changed;
        }

        protected override bool IsObjectValid()
        {
            return _aktieViewModel.Valid && CtrlEditKurse.Valid;
        }

        protected override void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
            UserEditings.InsertUserEditing(Constants.AktieEditControlID, this, modus, bo);
            _aktieViewModel.Update(bo, modus.IsModifying());
            CtrlEditKurse.StartEditingContent(bo.Id, !modus.IsModifying());
        }

        protected override void UpdateAfterEditing()
        {
            UserEditings.RemoveUserEditing(Constants.AktieEditControlID);
            _aktieViewModel.Update(BaseObject.CreateDefault<Aktie>(), false);
            CtrlEditKurse.FinishEditingContent(true);
            TabControlAktie.SelectedIndex = 0;
        }

        protected override void UpdateAfterSaving()
        {
            _aktieViewModel.Reset();
            CtrlEditKurse.FinishEditingContent(false);
        }
        #endregion

        #region Eventhandler
        private void ExecuteSaveAktie(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingAktie(false);
        }

        private void ExecuteSaveAktieAndClose(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingAktie(true);
        }

        private void CanExecuteSaveAktie(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveObjectAllowed();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            HandleCancelEditing(Properties.Resources.Title, Properties.Resources.AktieHasChanged);
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editing;
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save);
            cb.Executed += ExecuteSaveAktie;
            cb.CanExecute += CanExecuteSaveAktie;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.SaveAndClose);
            cb.Executed += ExecuteSaveAktieAndClose;
            cb.CanExecute += CanExecuteSaveAktie;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Stop);
            cb.Executed += ExecuteCancelEditing;
            cb.CanExecute += CanExecuteCancelEditing;
            CommandBindings.Add(cb);
        }

        private void FinishEditingAktie(bool close)
        {
            if (CtrlEditKurse.CheckKurse())
            {
                BaseObject aktie = _aktieViewModel.BaseObject;
                Kurse kurse = CtrlEditKurse.Kurse;
                var ado = new AktieDataObject(aktie, kurse);

                SaveObject(ado, close);
            }
        }

        private readonly AktieViewModel _aktieViewModel = null;
    }
}
