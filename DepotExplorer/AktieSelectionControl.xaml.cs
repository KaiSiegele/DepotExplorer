using Basics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control zur Auswahl von Aktien
    /// </summary>
    public partial class AktieSelectionControl : BaseObjectSelectionControl
    {
        public AktieSelectionControl()
        {
            InitializeComponent();

            CbLandSelection.ItemsSource = BaseObjectConverter.GetObjectNamesWithAll(MasterData.Laender.ObjectNames);
            _landSelectionViewModel = new LandSelectionViewModel();
            _landSelectionViewModel.Land = BaseObjectConverter.All;
            _landSelectionViewModel.PropertyChanged += OnSelectionChanged;
            CbLandSelection.DataContext = _landSelectionViewModel;

            _aktieViewModelms = new AktieViewModels(CachedData.Aktien);
            _aktieViewModelms.Land = BaseObjectConverter.All;
            LvwAktien.ItemsSource = _aktieViewModelms;
        }

        internal int SelectedLand
        {
            get
            {
                return _landSelectionViewModel.Land;
            }
        }

        internal Aktie SelectedAktie
        {
            get
            {
                return UserInterface.Tools.GetSelectedBaseObject<Aktie>(LvwAktien);
            }
        }

        #region Eventhandler
        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Land")
            {
                _aktieViewModelms.UpdateLand(_landSelectionViewModel.Land);
            }
        }

        void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                _aktieViewModelms.SortViewModels(headerClicked.Name);
            }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectSelected(SelectedAktie);
        }
        #endregion

        private readonly LandSelectionViewModel _landSelectionViewModel = null;
        private readonly AktieViewModels _aktieViewModelms = null;
    }
}
