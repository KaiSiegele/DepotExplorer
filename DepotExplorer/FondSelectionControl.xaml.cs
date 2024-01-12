using Basics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control zur Auswahl von Fonds
    /// </summary>
    public partial class FondSelectionControl : BaseObjectSelectionControl
    {
        public FondSelectionControl()
        {
            InitializeComponent();

            CbAnbieterSelection.ItemsSource = BaseObjectConverter.GetObjectNamesWithAll(MasterData.Anbieter.ObjectNames);
            _anbieterSelectionViewModel = new AnbieterSelectionViewModel();
            _anbieterSelectionViewModel.Anbieter = BaseObjectConverter.All;
            _anbieterSelectionViewModel.PropertyChanged += OnSelectionChanged;
            CbAnbieterSelection.DataContext = _anbieterSelectionViewModel;

            _fondViewModels = new FondViewModels(CachedData.Fonds);
            _fondViewModels.Anbieter = BaseObjectConverter.All;
            LvwFonds.ItemsSource = _fondViewModels;
        }

        internal int SelectedAnbieter
        {
            get
            {
                return _anbieterSelectionViewModel.Anbieter;
            }
        }

        internal Fond SelectedFond
        {
            get
            {
                return UserInterface.Tools.GetSelectedBaseObject<Fond>(LvwFonds);
            }
        }

        #region Eventhandler
        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Anbieter")
            {
                _fondViewModels.UpdateAnbieter(_anbieterSelectionViewModel.Anbieter);
            }
        }

        void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                _fondViewModels.SortViewModels(headerClicked.Name);
            }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ObjectSelected(SelectedFond);
        }
        #endregion

        private readonly AnbieterSelectionViewModel _anbieterSelectionViewModel = null;
        private readonly FondViewModels _fondViewModels = null;
    }
}
