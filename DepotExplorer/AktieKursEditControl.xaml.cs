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
    /// Control für die Neuerfassung, Änderung und Ansicht der Kurse der Aktien pro Jahr
    /// </summary>
    public partial class AktieKursEditControl : BaseContentEditControl, IObjectEditComponent
    {
        public AktieKursEditControl()
        {
            InitializeComponent();
            CbLaender.ItemsSource = MasterData.Laender.ObjectNames;

            _landSelectionViewModel = new LandSelectionViewModel();
            _landSelectionViewModel.Land = BaseObject.NotSpecified;
            _landSelectionViewModel.PropertyChanged += OnSelectionChanged;
            CbLaender.DataContext = _landSelectionViewModel;

            WertpapierAuswahl.ItemsSource = CachedData.Aktien.GetAktienFromLand(MasterData.DefaultLandId);
            _aktieKursViewModels = new AktieKursViewModels(_kurse, "Wertpapier");
            DataGridKurse.ItemsSource = _aktieKursViewModels;

            ClearControl(false);
            InitCommandBindings();
        }

        internal void PrepareEditingKurse(Kurse kurse)
        {
            var aktienKurse = kurse.GetKurseOfWertpapierArt(WertpapierArt.Aktie);
            _kurse.InsertObjects(aktienKurse);
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
                wpNames = CachedData.Aktien.GetObjectNames(doppelteWpIDs);
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
                return _aktieKursViewModels.Valid;
            }
        }

        public bool Changed
        {
            get
            {
                return _aktieKursViewModels.Changed;
            }

        }
        #endregion

        #region Methoden von BaseObjectContentControl
        protected override void UpdateBeforeEditing(bool isReadOnly)
        {
            WertpapierAuswahl.ItemsSource = CachedData.Aktien.GetAktienFromLand(MasterData.DefaultLandId);
            _landSelectionViewModel.Land = MasterData.DefaultLandId;
            CbLaender.IsEnabled = true;
            EnableKursGrid(true, isReadOnly);
        }

        protected override void UpdateAfterEditing(bool isReadOnly)
        {
            ClearControl(isReadOnly);
        }

        protected override void UpdateAfterSaving()
        {
            _aktieKursViewModels.ResetViewModels();
        }
        #endregion

        #region Eventhandler
        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Land")
            {
                _aktieKursViewModels.UpdateLand(_landSelectionViewModel.Land);
                var aktien = CachedData.Aktien.GetAktienFromLand(_landSelectionViewModel.Land).Select(f => f.Name);
                WertpapierAuswahl.ItemsSource = aktien;
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
            ObjectViewModel bovm = _aktieKursViewModels.GetObjectViewModel(id);
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
            _landSelectionViewModel.Land = BaseObject.NotSpecified;
            CbLaender.IsEnabled = false;
            EnableKursGrid(false, isReadOnly);
        }

        private void EnableKursGrid(bool enable, bool isReadOnly)
        {
            if (isReadOnly)
            {
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlWertpapier, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlWert, enable);
            }
            DataGridKurse.IsEnabled = enable;
        }

        private readonly LandSelectionViewModel _landSelectionViewModel = null;
        private readonly AktieKursViewModels _aktieKursViewModels = null;
        private readonly Kurse _kurse = new Kurse();
    }
}
