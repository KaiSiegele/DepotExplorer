using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Reportcontrol für den Jahresabschluss eines Depots.
    /// </summary>
    public partial class DepotJahresAbschlussReportControl : BaseReportControl
    {
        public DepotJahresAbschlussReportControl()
        {
            InitializeComponent();

            _depotViewModels = new DepotViewModels(CachedData.Depots);
            CbDepots.ItemsSource = _depotViewModels;
            _jahrViewModels = new JahrViewModels(CachedData.Aktien);
            CbJahre.ItemsSource = _jahrViewModels;
            _selectionViewModel = new DepotJahrSelectionViewModel { JahrId =CachedData.Jahre.DefaultId, Updated = true };
            GBSelection.DataContext = _selectionViewModel;

            _selectionViewModel.PropertyChanged += OnSelectionChanged;

            _report = new DepotJahresAbschlussgReport();
            DataGridJahresAbschlussPosten.ItemsSource = _report.DataTable.AsDataView();

            _summaryViewModel = new DepotJahresAbschlussSummaryViewModel();
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
            _report.JahrId = _selectionViewModel.JahrId;
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
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.LoadJahresAbschlussPosten);
            cb.Executed += ExecuteLoadPosten;
            cb.CanExecute += CanExecuteLoadPosten;
            CommandBindings.Add(cb);
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateReportMode(false);
        }

        private readonly DepotViewModels _depotViewModels = null;
        private readonly JahrViewModels _jahrViewModels = null;
        private readonly DepotJahrSelectionViewModel _selectionViewModel = null;
        private readonly DepotJahresAbschlussgReport _report = null;
        private readonly DepotJahresAbschlussSummaryViewModel _summaryViewModel = null;
    }
}
