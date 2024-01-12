using System.Linq;
using System.Windows.Input;
using UserInterface;
using VermoegensData;
using System.ComponentModel;
using Basics;
using System.Diagnostics;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht von Wertpapierkursen pro Jahr
    /// </summary>
    public partial class JahrEditControl : BaseObjectEditControl
    {
        public JahrEditControl()
        {
            InitializeComponent();

            _jahrViewModel = new JahrViewModel();
            GBJahr.DataContext = _jahrViewModel;
            _jahrViewModel.Update(BaseObject.CreateDefault<Jahr>());

            InitCommandBindings();
        }

        internal void StartUpdatingJahr(Jahr jahr, bool isModifying)
        {
            BaseObject jahr2Edit = jahr.Copy();
            Kurse kurse = new Kurse();
            var jdo = new JahrDataObject(jahr2Edit, kurse);

            if (jdo.Load())
            {
                CtrlEditFondKurse.PrepareEditingKurse(kurse);
                CtrlEditAktieKurse.PrepareEditingKurse(kurse);
                StartEditingObject(jahr2Edit, isModifying ? EditingModus.Updating : EditingModus.Viewing);
            }
        }

        #region Overrides von BaseEditControl
        public override BaseObject GetObject()
        {
            return _jahrViewModel.BaseObject;
        }

        protected override bool HasObjectChanged()
        {
            return CtrlEditFondKurse.Changed || CtrlEditAktieKurse.Changed;
        }

        protected override bool IsObjectValid()
        {
            return CtrlEditFondKurse.Valid && CtrlEditAktieKurse.Valid;
        }

        protected override void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
            bool readOnly = !modus.IsModifying();
            UserEditings.InsertUserEditing(Constants.JahrEditControlID, this, modus, bo);
            _jahrViewModel.Update(bo);
            CtrlEditFondKurse.StartEditingContent(bo.Id, readOnly);
            CtrlEditAktieKurse.StartEditingContent(bo.Id, readOnly);
        }

        protected override void UpdateAfterEditing()
        {
            UserEditings.RemoveUserEditing(Constants.JahrEditControlID);
            _jahrViewModel.Update(BaseObject.CreateDefault<Jahr>());
            CtrlEditFondKurse.FinishEditingContent(true);
            CtrlEditAktieKurse.FinishEditingContent(true);
        }
        protected override void UpdateAfterSaving()
        {
            CtrlEditFondKurse.FinishEditingContent(false);
            CtrlEditAktieKurse.FinishEditingContent(false);
        }
        #endregion

        #region Eventhandler
        private void ExecuteSaveKurse(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingKurse(false);
        }
        private void ExecuteSaveKurseAndClose(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingKurse(true);
        }

        private void CanExecuteSaveKurse(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveObjectAllowed();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            HandleCancelEditing(Properties.Resources.Title, Properties.Resources.KurseWereChanged);
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editing;
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save);
            cb.Executed += ExecuteSaveKurse;
            cb.CanExecute += CanExecuteSaveKurse;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.SaveAndClose);
            cb.Executed += ExecuteSaveKurseAndClose;
            cb.CanExecute += CanExecuteSaveKurse;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Stop);
            cb.Executed += ExecuteCancelEditing;
            cb.CanExecute += CanExecuteCancelEditing;
            CommandBindings.Add(cb);
        }

        private void FinishEditingKurse(bool close)
        {
            if (CtrlEditFondKurse.CheckKurse())
            {
                Debug.Assert(Modus == EditingModus.Updating);
                BaseObject jahr = _jahrViewModel.BaseObject;
                Kurse kurse = CtrlEditFondKurse.Kurse;
                Kurse aktienKurse = CtrlEditAktieKurse.Kurse;
                kurse.InsertObjects(aktienKurse);
                var jdo = new JahrDataObject(jahr, kurse);

                SaveObject(jdo, close);
            }
        }

        private readonly JahrViewModel _jahrViewModel = null;
    }
}
