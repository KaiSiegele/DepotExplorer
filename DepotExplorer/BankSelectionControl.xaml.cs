using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control zur Auswahl einer Bank
    /// </summary>
    public partial class BankSelectionControl : BaseObjectSelectionControl
    {
        public BankSelectionControl()
        {
            InitializeComponent();

            _bankViewModels = new BankViewModels(CachedData.Banken);
            LvwBanken.ItemsSource = _bankViewModels;
        }

        internal Bank SelectedBank
        {
            get { return UserInterface.Tools.GetSelectedBaseObject<Bank>(LvwBanken); }
        }

        #region Eventhandler
        private void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                _bankViewModels.SortViewModels(headerClicked.Name);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ObjectSelected(SelectedBank);
        }
        #endregion

        private readonly BankViewModels _bankViewModels = null;
    }
}
