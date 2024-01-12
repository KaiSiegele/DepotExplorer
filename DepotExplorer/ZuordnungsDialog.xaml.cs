using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using VermoegensData;
using Basics;
using UserInterface;
using Tools;

namespace DepotExplorer
{
    /// <summary>
    /// Eingabedialog fuer die Zuordnungen
    /// </summary>
    public partial class ZuordnungsDialog : Window
    {
        public ZuordnungsDialog(BaseObject bo, Zuordnungen zuordnungen, Bestaende bestaende)
        {
            InitializeComponent();

            Title = Properties.Resources.ResourceManager.GetMessageFromResource("TitleZuordnungsDialog", bo.ToString());

            _bo = bo;
            _bestaende = bestaende;

            _zuordnungViewModels = new ZuordnungViewModels(_zuordnungen);
            _zuordnungen.InsertObjects(zuordnungen);
            LbZuordnungen.ItemsSource = _zuordnungViewModels;

            _wpaSelectionViewModel = new WertpapierArtSelectionViewModel();
            _wpaSelectionViewModel.WertpapierArt = WertpapierArt.Fond;
            _wpaSelectionViewModel.PropertyChanged += OnSelectionChanged;
            UserInterface.Tools.FillCombox<WertpapierArt>(CbWertpapierArt, false);
            CbWertpapierArt.DataContext = _wpaSelectionViewModel;

            BaseObjects zwp = WertpapierInfo.GetWertpapiere();
            _zwpViewModels = new ZuordnebaresWertpapierViewModels(zwp, _zuordnungen);
            LbZuordnebareWertpapiere.ItemsSource = _zwpViewModels;
            _zwpViewModels.UpdateWertpapierArt(WertpapierArt.Fond);
 
            InitCommandBindings();
        }

        public Zuordnungen Zuordnungen
        {
            get
            {
                return _zuordnungen;
            }
        }
 
        #region Eventhandler
        private void ExecuteSaveZuordnungen(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CanExecuteSaveZuordnungen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _zuordnungViewModels.CanSave();
        }

        private void ExecuteCancelEditing(object sender, ExecutedRoutedEventArgs e)
        {
            bool cancel;
            if (_zuordnungViewModels.Changed)
            {
                cancel = Prompts.AskUser(Properties.Resources.Title, "{0}.{1}", Properties.Resources.ZuordnungenWereChanged, Properties.Resources.CancelEditing);
            }
            else
            {
                cancel = true;
            }
            if (cancel)
            {
                DialogResult = false;
                Close();
            }
        }

        private void CanExecuteCancelEditing(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecuteInsertZuordnung(object sender, ExecutedRoutedEventArgs e)
        {
            InsertZuordnung();
        }

        private void CanExecuteInsertZuordnung(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (GetSelectedWertpapier() != null);
        }

        private void ExecuteRemoveZuordnung(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveZuordnung();
        }

        private void CanExecuteRemoveZuordnung(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRemoveZuordnung();
        }

        private void OnSelectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "WertpapierArt")
            {
                _zwpViewModels.UpdateWertpapierArt(_wpaSelectionViewModel.WertpapierArt);
            }
        }
        #endregion

        #region Zuordnungen
        private void InsertZuordnung()
        {
            Wertpapier wp = GetSelectedWertpapier();
            if (wp != null)
            {
                _zwpViewModels.RemoveViewModel(wp.Id);
                _zuordnungen.InsertZuordnung(_bo.Id, wp.Id);
            }
        }

        private Wertpapier GetSelectedWertpapier()
        {
            return UserInterface.Tools.GetSelectedBaseObject<Wertpapier>(LbZuordnebareWertpapiere);
        }

        private Zuordnung GetSelectedZuordnung()
        {
            return UserInterface.Tools.GetSelectedBaseObject<Zuordnung>(LbZuordnungen);
        }

        private bool CanRemoveZuordnung()
        {
            bool result;
            Zuordnung zu = GetSelectedZuordnung();
            if (zu != null)
            {
                result = (_bestaende.HasWertpapierBestaende(zu.Wertpapier) == false);
            }
            else
            {
                result = false;
            }
            return result;
        }

        private void RemoveZuordnung()
        {
            Zuordnung zu = GetSelectedZuordnung();
            if (zu != null)
            {
                _zuordnungen.RemoveZuordnung(zu.Id);
                _zwpViewModels.InsertViewModel(zu.Wertpapier);
            }
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save);
            cb.Executed += ExecuteSaveZuordnungen;
            cb.CanExecute += CanExecuteSaveZuordnungen;
            CommandBindings.Add(cb);

            cb = new CommandBinding(ApplicationCommands.Stop);
            cb.Executed += ExecuteCancelEditing;
            cb.CanExecute += CanExecuteCancelEditing;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.InsertZuordnung);
            cb.Executed += ExecuteInsertZuordnung;
            cb.CanExecute += CanExecuteInsertZuordnung;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.RemoveZuordnung);
            cb.Executed += ExecuteRemoveZuordnung;
            cb.CanExecute += CanExecuteRemoveZuordnung;
            CommandBindings.Add(cb);
        }

        private readonly WertpapierArtSelectionViewModel _wpaSelectionViewModel = null;
        private readonly ZuordnebaresWertpapierViewModels _zwpViewModels = null;
        private readonly ZuordnungViewModels _zuordnungViewModels = null;

        private readonly BaseObject _bo = null;
        private readonly Zuordnungen _zuordnungen = new Zuordnungen();
        private readonly Bestaende _bestaende = null;
    }
}
