using Basics;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Reportcontrol, dass eine Übersicht über alle Depots in einem Jahr anzeigt
    /// </summary>
    public partial class JahresUebersichtReportControl : BaseReportControl
    {
        public JahresUebersichtReportControl()
        {
            InitializeComponent();
            
            _jahrViewModels = new JahrViewModels(CachedData.Jahre);
            CbJahre.ItemsSource = _jahrViewModels;
            _selectionViewModel = new JahrSelectionViewModel();
            _selectionViewModel.JahrId = BaseObject.NotSpecified;
            _selectionViewModel.Updated = false;

            GBSelection.DataContext = _selectionViewModel;

            _selectionViewModel.PropertyChanged += OnSelectionChanged;

            _report = new JahresUebersichtReport();
            DataGridDepotUebersicht.ItemsSource = _report.DataTable.AsDataView();

            _summaryViewModel = new JahresUebersichtSummaryViewModel();
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
        private void ExecuteLoadReport(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateReportMode(true);
        }

        private void CanExecuteLoadReport(object sender, CanExecuteRoutedEventArgs e)
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
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.LoadDepotUebersichtPosten);
            cb.Executed += ExecuteLoadReport;
            cb.CanExecute += CanExecuteLoadReport;
            CommandBindings.Add(cb);
        }

        private readonly JahrViewModels _jahrViewModels = null;
        private readonly JahrSelectionViewModel _selectionViewModel = null;
        private readonly JahresUebersichtReport _report = null;
        private readonly SummaryViewModel _summaryViewModel = null;
    }
}
