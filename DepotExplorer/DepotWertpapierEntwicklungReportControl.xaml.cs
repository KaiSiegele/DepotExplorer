using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Reportcontrol, dass die Entwicklung eines Wertpapiers in einem Depot über die Jahre anzeigt
    /// </summary>
    public partial class DepotWertpapierEntwicklungReportControl : BaseReportControl
    {
        public DepotWertpapierEntwicklungReportControl()
        {
            InitializeComponent();

            _depotViewModels = new DepotViewModels(CachedData.Depots);
            CbDepots.ItemsSource = _depotViewModels;
            _selectionViewModel = new DepotWertpapierSelectionViewModel { Updated = true };
            GBSelection.DataContext = _selectionViewModel;

            _selectionViewModel.PropertyChanged += OnSelectionChanged;

            _report = new DepotWertpapierEntwicklungReport();
            DataGridWertpapierEntwicklungPosten.ItemsSource = _report.DataTable.AsDataView();

            _summaryViewModel = new WertpapierEntwicklungSummaryViewModel();
            CtrlSummary.DataContext = _summaryViewModel;

            InitCommandBindings();
        }

        #region Overrides von der Basisklasse

        protected override void ClearReport()
        {
            _report.ClearTable();
            _summaryViewModel.Reset();
        }

        protected override bool LoadReport()
        {
            bool result = false;
            _report.DepotId = _selectionViewModel.DepotId;
            _report.WertpapierId = _selectionViewModel.WertpapierId;

            if (_report.FillTable())
            {
                _summaryViewModel.Update(_report.DataTable);
                _selectionViewModel.Updated = false;
                result = true;
            }
            return result;
        }
        #endregion

        #region CommmandBindings
        private void ExecuteLoadPosten(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateReportMode(true);
        }

        private void CanExecuteLoadPosten(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _selectionViewModel.IsValid() && _selectionViewModel.Updated;
        }
        #endregion
        
        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.LoadFondEntwicklungPosten);
            cb.Executed += ExecuteLoadPosten;
            cb.CanExecute += CanExecuteLoadPosten;
            CommandBindings.Add(cb);
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Depot")
            {
                CbWertpapiere.ItemsSource = _selectionViewModel.Wertpapiere.ObjectNames;
            }
            UpdateReportMode(false);
        }

        private readonly DepotWertpapierSelectionViewModel _selectionViewModel = null;
        private readonly DepotViewModels _depotViewModels = null;
        private readonly DepotWertpapierEntwicklungReport _report = null;
        private readonly WertpapierEntwicklungSummaryViewModel _summaryViewModel = null;
    }
}
