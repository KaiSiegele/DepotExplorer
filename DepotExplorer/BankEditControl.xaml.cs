using Basics;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht von Banken
    /// </summary>
    public partial class BankEditControl : BaseObjectEditControl
    {
        public BankEditControl()
        {
            InitializeComponent();

            _bankViewModel = new BankViewModel();
            _addressViewModel = new AddressViewModel();

            GBBank.DataContext = _bankViewModel;
            GBAddresse.DataContext = _addressViewModel;

            ClearControl();
            InitCommandBindings();
        }

        internal void StartInsertingBank()
        {
            string title = Properties.Resources.Title;
            Bank bank = Bank.CreateBank();
            Addresse addresse = Addresse.CreateAddresse(bank.Id, OwnerType.Bank);
            if (CheckNewObject(title, Properties.Resources.Bank, bank) && CheckNewObject(title, Properties.Resources.Addresse, addresse))
            {
                _addresse = addresse;
                StartEditingObject(bank, EditingModus.Inserting);
            }
        }

        internal void StarEditingBank(Bank bank, bool isModifying)
        {
            BaseObject bank2Edit = bank.Copy();
            _addresse = BaseObject.CreateDefault<Addresse>();
            _addresse.OwnerType = OwnerType.Bank;
            var bdo = new BankDataObject(bank2Edit, _addresse);

            if (bdo.Load())
            {
                StartEditingObject(bank2Edit, isModifying ? EditingModus.Updating : EditingModus.Viewing);
            }
            else
            {
                _addresse = BaseObject.CreateDefault<Addresse>();
            }
        }

        #region Overrides von BaseEditControl
        public override BaseObject GetObject()
        {
            return _bankViewModel.BaseObject;
        }

        protected override bool HasObjectChanged()
        {
            return _bankViewModel.Changed || _addressViewModel.Changed;
        }

        protected override bool IsObjectValid()
        {
            return _bankViewModel.Valid && _addressViewModel.Valid;
        }

        protected override void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
            UserEditings.InsertUserEditing(Constants.BankEditControlID, this, modus, bo);
            _bankViewModel.Update(bo, modus.IsModifying());
            _addressViewModel.Update(_addresse, modus.IsModifying());
        }

        protected override void UpdateAfterEditing()
        {
            UserEditings.RemoveUserEditing(Constants.BankEditControlID);
            ClearControl();
        }

        protected override void UpdateAfterSaving()
        {
            _bankViewModel.Reset();
            _addressViewModel.Reset();
        }
        #endregion

        #region Eventhandler
        private void ExecuteSaveDepot(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingBank(false);
        }

        private void ExecuteSaveDepotAndClose(object sender, ExecutedRoutedEventArgs e)
        {
            FinishEditingBank(true);
        }

        private void CanExecuteSaveDepot(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveObjectAllowed();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            HandleCancelEditing(Properties.Resources.Title, Properties.Resources.BankHasChanged);
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editing;
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
        }

        void FinishEditingBank(bool close)
        {
            BaseObject bank = _bankViewModel.BaseObject;
            Addresse addresse = _addressViewModel.BaseObject as Addresse;
            var bdo = new BankDataObject(bank, addresse);

            SaveObject(bdo, close);
        }

        private void ClearControl()
        {
            _addresse = BaseObject.CreateDefault<Addresse>();
            _bankViewModel.Update(BaseObject.CreateDefault<Bank>(), false);
            _addressViewModel.Update(_addresse, false);
        }

        private Addresse _addresse = null;

        private readonly BankViewModel _bankViewModel = null;
        private readonly AddressViewModel _addressViewModel = null;
    }
}
