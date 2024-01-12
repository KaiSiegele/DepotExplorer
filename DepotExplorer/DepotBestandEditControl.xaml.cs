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
    ///  Control für die Neuerfassung, Änderung und Ansicht der Bestaende der Depots
    /// </summary>
    public partial class DepotBestandEditControl : BaseContentEditControl, IObjectEditComponent
    {
        public DepotBestandEditControl()
        {
            InitializeComponent();

            _bestandViewModels = new BestandViewModels(_bestaende);
            _bestandViewModels.Jahr = BaseObject.NotSpecified;
            DataGridBestaende.ItemsSource = _bestandViewModels;

            _jahrViewModels = new JahrViewModels(CachedData.Jahre);
            CbJahre.ItemsSource = _jahrViewModels;
            _jahrSelectionViewModel = new JahrSelectionViewModel();
            _jahrSelectionViewModel.JahrId = BaseObject.NotSpecified;
            _jahrSelectionViewModel.PropertyChanged += OnSelectionChanged;
            CbJahre.DataContext = _jahrSelectionViewModel;

            ClearControl(false);
            InitCommandBindings();
        }

        internal void PrepareEditingBestaende(Bestaende bestaende, IEnumerable<int> selectableWertpapiere)
        {
            _selectableWertpapiere = selectableWertpapiere.ToList();
            _bestaende.InsertObjects(bestaende);
        }

        internal void UpdateSelectableWertpapiere(IEnumerable<int> selectableWertpapiere)
        {
            _selectableWertpapiere = selectableWertpapiere.ToList();
            WertpapierAuswahl.ItemsSource = WertpapierInfo.GetNames(_selectableWertpapiere);
        }

        internal bool CheckBestaende()
        {
            bool result = true;
            List<int> doppelteJahrIds = new List<int>();

            if (!_bestaende.CheckWertpapiereUnique(doppelteJahrIds))
            {
                List<string> jahrNames = new List<string>();
                jahrNames = CachedData.Jahre.GetObjectNames(doppelteJahrIds);
                Prompts.ShowInfo(Properties.Resources.Title, Properties.Resources.WertpapierBestandDoppelt, Misc.GetNamesAsString(jahrNames));
                result = false;
            }
            return result;
        }

        internal Bestaende Bestaende
        {
            get
            {
                return _bestaende;
            }
        }

        #region Methoden von BaseObjectContentControl
        protected override void UpdateBeforeEditing(bool isReadOnly)
        {
            EnableBestandsSelection(true);

            WertpapierAuswahl.ItemsSource = WertpapierInfo.GetNames(_selectableWertpapiere);
            EnableBestandGrid(true, isReadOnly);
        }

        protected override void UpdateAfterEditing(bool isReadOnly)
        {
            ClearControl(isReadOnly);
        }

        protected override void UpdateAfterSaving()
        {
            _bestandViewModels.ResetViewModels();
        }
        #endregion

        #region Interface IEditable Compontent
        public bool Valid
        {
            get
            {
                return _bestandViewModels.Valid;
            }
        }

        public bool Changed
        {
            get
            {
                return _bestandViewModels.Changed;
            }

        }
        #endregion

        #region Eventhandler

        private void ExecuteRemoveBestand(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveBestand();
        }

        private void CanExecuteRemoveBestand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (GetSelectedBestand() != null);
        }

        private void ExecuteInsertBestand(object sender, ExecutedRoutedEventArgs e)
        {
            InsertBestand();
        }

        private void CanExecuteInsertBestand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (EditContentAllowed() && _selectableWertpapiere.Count() > 0);
        }

        private void ExecuteExtendBestaende(object sender, ExecutedRoutedEventArgs e)
        {
            ExtendBestaende();
        }

        private void CanExecuteExtendBestaende(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (EditContentAllowed() && _selectableWertpapiere.Count() > 0);
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Jahr")
            {
                _bestandViewModels.UpdateJahr(_jahrSelectionViewModel.JahrId);
            }
        }
        #endregion

        private Bestand GetSelectedBestand()
        {
            return UserInterface.Tools.GetSelectedBaseObject<Bestand>(DataGridBestaende);
        }

        private void SelectBestand(int id)
        {
            ObjectViewModel bovm = _bestandViewModels.GetObjectViewModel(id);
            if (bovm != null)
            {
                DataGridBestaende.SelectedItem = bovm;
            }
        }

        private void InsertBestand()
        {
            int id = 0;
            if (_bestaende.InsertBestand(ObjectId, _bestandViewModels.Jahr, ref id))
            {
                SelectBestand(id);
            }
        }

        private void RemoveBestand()
        {
            Bestand bestand = GetSelectedBestand();
            if (bestand != null)
            {
                _bestaende.RemoveBestand(bestand.Id);
            }
        }

        private void ExtendBestaende()
        {
           _bestaende.InsertBestaendeForMissingWertpapiere(ObjectId, _jahrSelectionViewModel.JahrId, _selectableWertpapiere);
        }
                
        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.InsertBestand);
            cb.Executed += ExecuteInsertBestand;
            cb.CanExecute += CanExecuteInsertBestand;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.RemoveBestand);
            cb.Executed += ExecuteRemoveBestand;
            cb.CanExecute += CanExecuteRemoveBestand;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ExtendBestaende);
            cb.Executed += ExecuteExtendBestaende;
            cb.CanExecute += CanExecuteExtendBestaende;
            CommandBindings.Add(cb);
        }

        private void ClearControl(bool isReadOnly)
        {
            _bestaende.ClearObjects();
            _selectableWertpapiere.Clear();

            EnableBestandsSelection(false);
            EnableBestandGrid(false, isReadOnly);
        }

        private void EnableBestandsSelection(bool enable)
        {
            int jahrid = enable ? CachedData.Jahre.DefaultId : BaseObject.NotSpecified;
            _jahrSelectionViewModel.JahrId = jahrid;
            _jahrSelectionViewModel.Enabled = enable;
            if (enable)
            {
                CbJahre.SelectedItem = _jahrViewModels.GetViewModel<JahrViewModel>(jahrid);
            }
            else
            {
                CbJahre.SelectedItem = null;
                CbJahre.Text = string.Empty;
            }
            _bestandViewModels.UpdateJahr(jahrid);
        }

        private void EnableBestandGrid(bool enable, bool isReadOnly)
        {
            if (isReadOnly)
            {
                UserInterface.Tools.SetColumnReadOnly(DataGridBestaende, Properties.Resources.HeaderBestandControlWertpapier, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridBestaende, Properties.Resources.HeaderBestandControlAnteile, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridBestaende, Properties.Resources.HeaderBestandControlKauf, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridBestaende, Properties.Resources.HeaderBestandControlVerkauf, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridBestaende, Properties.Resources.HeaderBestandControlDividende, enable);
            }
            DataGridBestaende.IsEnabled = enable;
        }

        private List<int> _selectableWertpapiere = new List<int>();
        private readonly BestandViewModels _bestandViewModels = null;
        private readonly JahrViewModels _jahrViewModels = null;
        private readonly JahrSelectionViewModel _jahrSelectionViewModel = null;
        private readonly Bestaende _bestaende = new Bestaende();
    }
}
