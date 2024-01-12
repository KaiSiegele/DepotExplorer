using Basics;
using System.Collections.Generic;
using System.Windows.Input;
using Tools;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// Control für die Neuerfassung, Änderung und Ansicht der Kurse eines Wertpapiers
    /// </summary>
    public partial class WertpapierKursEditControl : BaseContentEditControl, IObjectEditComponent
    {
        public WertpapierKursEditControl()
        {
            InitializeComponent();

            _kursViewModels = new WertpapierKursViewModels(_kurse, "Jahr");
            DataGridKurse.ItemsSource = _kursViewModels;
            ClearControl(false);
            InitCommandBindings();
        }

        internal void PrepareEditingKurse(Kurse kurse)
        {
            _kurse.InsertObjects(kurse);
        }

        internal Kurse Kurse
        {
            get
            {
                return _kurse;
            }
        }

        internal bool CheckKurse()
        {
            bool result = false;
            List<int> doppelteJahrIds = new List<int>();
            if (_kurse.CheckJahreUnique(doppelteJahrIds))
            {
                if (_kurse.CheckJahreLueckenlos())
                {
                    result = true;
                }
                else
                {
                    Prompts.ShowInfo(Properties.Resources.Title, Properties.Resources.JahresKurseNichtLueckenlos);
                }
            }
            else
            {
                List<string> jahrNames = new List<string>();
                jahrNames =CachedData.Jahre.GetObjectNames(doppelteJahrIds);
                Prompts.ShowInfo(Properties.Resources.Title, Properties.Resources.JahresKursDoppelt, Misc.GetNamesAsString(jahrNames));
            }
            return result;
        }

        #region Methoden von BaseObjectContentControl
        protected override void UpdateBeforeEditing(bool isReadOnly)
        {
            List<string> lstJahre =CachedData.Jahre.ObjectNames;
            JahrAuswahl.ItemsSource = lstJahre;
            EnableKursGrid(true, isReadOnly);
        }

        protected override void UpdateAfterEditing(bool isReadOnly)
        {
            ClearControl(isReadOnly);
        }

        protected override void UpdateAfterSaving()
        {
            _kursViewModels.ResetViewModels();
        }
        #endregion
 
        #region Interface IEditable Compontent
        public bool Valid
        {
            get
            {
                return _kursViewModels.Valid;
            }
        }

        public bool Changed
        {
            get
            {
                return _kursViewModels.Changed;
            }

        }
        #endregion

        #region Eventhandler
        private void ExecuteRemoveKurs(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveKurs();
        }

        private void CanExecuteRemoveKurs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (EditContentAllowed() && GetSelectedKurs() != null);
        }

        private void ExecuteInsertKurs(object sender, ExecutedRoutedEventArgs e)
        {
            InsertKurs();
        }

        private void CanExecuteInsertKurs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditContentAllowed();
        }

        private void ExecuteExtendKurse(object sender, ExecutedRoutedEventArgs e)
        {
            ExtendKurse();
        }

        private void CanExecuteExtendKurse(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditContentAllowed();
        }
        #endregion

        private void InitCommandBindings()
        {
            CommandBinding cb = new CommandBinding(VermoegensExplorerCommands.InsertKursWert);
            cb.Executed += ExecuteInsertKurs;
            cb.CanExecute += CanExecuteInsertKurs;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.RemoveKursWert);
            cb.Executed += ExecuteRemoveKurs;
            cb.CanExecute += CanExecuteRemoveKurs;
            CommandBindings.Add(cb);

            cb = new CommandBinding(VermoegensExplorerCommands.ExtendKursWerte);
            cb.Executed += ExecuteExtendKurse;
            cb.CanExecute += CanExecuteExtendKurse;
            CommandBindings.Add(cb);
        }

        private void ClearControl(bool isReadOnly)
        {
            _kurse.ClearObjects();
            JahrAuswahl.ItemsSource = null;
            EnableKursGrid(false, isReadOnly);
        }

        private void EnableKursGrid(bool enable, bool isReadOnly)
        {
            if(isReadOnly)
            {
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlJahr, enable);
                UserInterface.Tools.SetColumnReadOnly(DataGridKurse, Properties.Resources.HeaderKursControlWert, enable);
            }
            DataGridKurse.IsEnabled = enable;
        }

        #region Kurse
        private void InsertKurs()
        {
            int id = 0;
            if (_kurse.InsertKurs(ObjectId, BaseObject.NotSpecified, ref id))
            {
                SelectKurs(id);
            }
        }

        private void RemoveKurs()
        {
            Kurs kurs = GetSelectedKurs();
            if (kurs != null)
            {
                _kurse.RemoveKurs(kurs.Id);
            }
        }

        private void ExtendKurse()
        {
            _kurse.InsertKurseForMissingJahre(ObjectId,CachedData.Jahre);
        }

        private Kurs GetSelectedKurs()
        {
            return UserInterface.Tools.GetSelectedBaseObject<Kurs>(DataGridKurse);
        }

        private void SelectKurs(int id)
        {
            ObjectViewModel bovm = _kursViewModels.GetObjectViewModel(id);
            if (bovm != null)
            {
                DataGridKurse.SelectedItem = bovm;
            }
        }
        #endregion

        private WertpapierKursViewModels _kursViewModels = null;
        private readonly Kurse _kurse = new Kurse();
   }
}
