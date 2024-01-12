using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Auswahl der Jahre
    /// </summary>
    public partial class JahrSelectionControl : BaseObjectSelectionControl
    {
        public JahrSelectionControl()
        {
            InitializeComponent();

            _jahrViewModels = new JahrViewModels(CachedData.Jahre);
            LvwJahre.ItemsSource = _jahrViewModels;
        }

        public Jahr SelectedJahr
        {
            get
            {
                return UserInterface.Tools.GetSelectedBaseObject<Jahr>(LvwJahre);
            }
        }

        #region Eventhandler
        void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                _jahrViewModels.SortViewModels(headerClicked.Name);
            }
        }
        
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ObjectSelected(SelectedJahr);
        }
        #endregion

        private readonly JahrViewModels _jahrViewModels = null;
    }
}