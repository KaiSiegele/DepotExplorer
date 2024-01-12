using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control zur Auswahl eines Depots
    /// </summary>
    public partial class DepotSelectionControl : BaseObjectSelectionControl
    {
        public DepotSelectionControl()
        {
            InitializeComponent();

            _depotViewModels = new DepotViewModels(CachedData.Depots);
            LvwDepots.ItemsSource = _depotViewModels;
        }

        public Depot SelectedDepot
        {
            get
            {
                return UserInterface.Tools.GetSelectedBaseObject<Depot>(LvwDepots);
            }
        }

        #region Eventhandler
        void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                _depotViewModels.SortViewModels(headerClicked.Name);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ObjectSelected(SelectedDepot);
        }
        #endregion

        private readonly DepotViewModels _depotViewModels = null;
    }
}
