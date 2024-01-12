using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Reportcontrol, das die Entwicklung eines Depots über die Jahre anzeigt
    /// </summary>
    public partial class DepotEntwicklungReportControl : BaseReportControl
    {
        public DepotEntwicklungReportControl()
        {
            InitializeComponent();

            _deoptViewMdodels = new DepotViewModels(CachedData.Depots);
            CbDepots.ItemsSource = _deoptViewMdodels;
            _selectionViewModel = new DepotSelectionViewModel();
            
            GBSelection.DataContext = _selectionViewModel;
            _selectionViewModel.PropertyChanged += OnSelectionChanged;

            _report = new DepotEntwicklungReport();
            DataGridDepotEntwicklungPosten.ItemsSource = _report.DataTable.AsDataView();

            _summaryVieModel = new DepotEntwicklungSummaryViewModel();
            CtrlSummary.DataContext = _summaryVieModel;
  
            InitCommandBindings();
        }

        #region Overrides von der Basisklasse
        protected override void ClearReport()
        {
            _report.ClearTable();
            _summaryVieModel.Reset();
        }

        protected override bool LoadReport()
        {
            bool result = false;
            _report.DepotId = _selectionViewModel.DepotId;
            if (_report.FillTable())
            {
                _summaryVieModel.Update(_report.DataTable);
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

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateReportMode(false);
        }

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.LoadDepotEntwicklungPosten);
            cb.Executed += ExecuteLoadPosten;
            cb.CanExecute += CanExecuteLoadPosten;
            CommandBindings.Add(cb);
        }

        private readonly DepotViewModels _deoptViewMdodels = null;
        private readonly DepotSelectionViewModel _selectionViewModel = null;
        private readonly DepotEntwicklungReport _report = null;
        private readonly SummaryViewModel _summaryVieModel = null;
    }
}
