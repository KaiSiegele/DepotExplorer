using Basics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Tools;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht der Kurse der Fonds pro Jahr
    /// </summary>
    public partial class FondKursEditControl : BaseContentEditControl, IObjectEditComponent
    {
        public FondKursEditControl()
        {
            InitializeComponent();
            CbAnbieter.ItemsSource = MasterData.Anbieter.ObjectNames;

            _anbieterSelectionViewModel = new AnbieterSelectionViewModel();
            _anbieterSelectionViewModel.Anbieter = BaseObject.NotSpecified;
            _anbieterSelectionViewModel.PropertyChanged += OnSelectionChanged;
            CbAnbieter.DataContext = _anbieterSelectionViewModel;

            _fondKursViewModels = new FondKursViewModels(_kurse, "Wertpapier");
            DataGridKurse.ItemsSource = _fondKursViewModels;

            ClearControl(false);
            InitCommandBindings();
        }

        internal void PrepareEditingKurse(Kurse kurse)
        {
            var fondKurse = kurse.GetKurseOfWertpapierArt(WertpapierArt.Fond);
            _kurse.InsertObjects(fondKurse);
        }

        internal Kurse Kurse
        {
            get
            {
                return _kurse;
            }
        }

        internal bool CheckKurse()
        {
            List<int> doppelteWpIDs = new List<int>();
            if (!_kurse.CheckWertpapiereUnique(doppelteWpIDs))
            {
                List<string> wpNames = new List<string>();
                wpNames = CachedData.Fonds.GetObjectNames(doppelteWpIDs);
                Prompts.ShowInfo(Properties.Resources.Title, Properties.Resources.WertpapierKursDoppelt, Misc.GetNamesAsString(wpNames));
                return false;
            }
            return true;
        }

        #region Interface IEditable Compontent
        public bool Valid
        {
            get
            {
                return _fondKursViewModels.Valid;
            }
        }

        public bool Changed
        {
            get
            {
                return _fondKursViewModels.Changed;
            }

        }
        #endregion

        #region Methoden von BaseObjectContentControl
        protected override void UpdateBeforeEditing(bool isReadOnly)
        {
            WertpapierAuswahl.ItemsSource = CachedData.Fonds.GetFondsFromAnbieter(MasterData.DefaultAnbieterId);
            _anbieterSelectionViewModel.Anbieter = MasterData.DefaultAnbieterId;
            CbAnbieter.IsEnabled = true;
            EnableKursGrid(true, isReadOnly);
        }

        protected override void UpdateAfterEditing(bool isReadOnly)
        {
            ClearControl(isReadOnly);
        }

        protected override void UpdateAfterSaving()
        {
            _fondKursViewModels.ResetViewModels();
        }
        #endregion

        #region Eventhandler
        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Anbieter")
            {
                _fondKursViewModels.UpdateAnbieter(_anbieterSelectionViewModel.Anbieter);
                WertpapierAuswahl.ItemsSource = CachedData.Fonds.GetFondsFromAnbieter(_anbieterSelectionViewModel.Anbieter).Select(f => f.Name);
            }
        }

        private void ExecuteRemoveKurs(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveKurs();
        }

        private void CanExecuteRemoveKurs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (EditContentAllowed() && GetSelectedKurs() != null);
        }

        private void ExecuteInsertKurs(object sender, ExecutedRoutedEventArgs e)
        {
            InsertKurs();
        }

        private void CanExecuteInsertKurs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditContentAllowed();
        }
        #endregion

        #region Kurse
        private void InsertKurs()
        {
            int id = 0;
            if (_kurse.InsertKurs(BaseObject.NotSpecified, ObjectId, ref id))
            {
                SelectKurs(id);
            }
        }

        private void RemoveKurs()
        {
            Kurs kurs = GetSelectedKurs();
            if (kurs != null)
            {
                _kurse.RemoveKurs(kurs.Id);
            }
        }

        private Kurs GetSelectedKurs()
        {
            return UserInterface.Tools.GetSelectedBaseObject<Kurs>(DataGridKurse);
        }

        private void SelectKurs(int id)
        {
            ObjectViewModel bovm = _fondKursViewModels.GetObjectViewModel(id);
            if (bovm != null)
            {
                DataGridKurse.SelectedItem = bovm;
            }
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.InsertKursWert);
            cb.Executed += ExecuteInsertKurs;
            cb.CanExecute += CanExecuteInsertKurs;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.RemoveKursWert);
            cb.Executed += ExecuteRemoveKurs;
            cb.CanExecute += CanExecuteRemoveKurs;
            CommandBindings.Add(cb);
        }

        private void ClearControl(bool isReadOnly)
        {
            _kurse.ClearObjects();
            _anbieterSelectionViewModel.Anbieter = BaseObject.NotSpecified;
            CbAnbieter.IsEnabled = false;
            EnableKursGrid(false, isReadOnly);
        }

        void EnableKursGrid(bool enable, bool isReadOnly)
        {
            if (isReadOnly)
            {
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlWertpapier, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlWert, enable);
            }
            DataGridKurse.IsEnabled = enable;
        }

        private readonly AnbieterSelectionViewModel _anbieterSelectionViewModel = null;
        private readonly FondKursViewModels _fondKursViewModels = null;
        private readonly Kurse _kurse = new Kurse();

    }
}
