using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Basics;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für Viewmodels, die die Daten 
    /// Modells (Objekt der Klasse BaseObject) anzeigen
    /// und auswaehlbar sind
    /// 
    /// </summary>
    public class SelectabeBaseObjectViewModel : ObjectViewModel
    {
        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="bo">Modell für die Datenanzeige</param>
        public SelectabeBaseObjectViewModel(BaseObject bo) : base(bo)
        {
        }

        #region Eigenschaften
        public string Name
        {
            get
            {
                return _bo.Name;
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
                Changed = true;
            }
        }
        private bool _selected;

        public bool Changed
        {
            get
            {
                return _changed;
            }
            set
            {
                _changed = value;
                OnPropertyChanged("Changed");
            }
        }
        private bool _changed;
        #endregion

        #region Methoden der Basisklasse
        public override bool HasChanged()
        {
            return Changed;
        }

        public override bool IsValid()
        {
            return true;
        }

        protected override void Refresh()
        {
            _changed = false;
            _selected = false;

            IEnumerable<PropertyInfo> props = GetType().GetProperties();
            foreach (var pi in props)
            {
                OnPropertyChanged(pi.Name);
            }
        }
        #endregion
    }
    public class SelectableListViewModel : ObjectViewModels
    {
        public SelectableListViewModel(BaseObjects bos, string sortCriterium)
          : base(bos, sortCriterium, false)
        {
            RefreshViewModels();
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new SelectabeBaseObjectViewModel(baseObject);
        }

        public List<int> GetSelectedObjectIds()
        {
            return (from sbovm in this.OfType<SelectabeBaseObjectViewModel>() where sbovm.Selected select sbovm.ObjectId).ToList<int>();
        }

        public void SelectObjects(List<int> ids, bool selected)
        {
            foreach (int id in ids)
            {
                SelectabeBaseObjectViewModel sobvm = GetObjectViewModel(id) as SelectabeBaseObjectViewModel;
                if (sobvm != null)
                {
                    sobvm.Selected = selected;
                }
            }
        }

        public void SelectAllObjects(bool selected)
        {
            IEnumerable<SelectabeBaseObjectViewModel> sobvms = (from sbovm in this.OfType<SelectabeBaseObjectViewModel>() select sbovm);
            foreach (SelectabeBaseObjectViewModel sobvm in sobvms)
            {
                sobvm.Selected = selected;
            }
        }

        public void SetAllUnchanged()
        {
            IEnumerable<SelectabeBaseObjectViewModel> sobvms = (from sbovm in this.OfType<SelectabeBaseObjectViewModel>() select sbovm);
            foreach (SelectabeBaseObjectViewModel sobvm in sobvms)
            {
                sobvm.Changed = false;
            }
        }

        public bool CanQuery()
        {
            if (Changed)
            {
                return (from sbovm in this.OfType<SelectabeBaseObjectViewModel>() where sbovm.Selected select sbovm).Any<SelectabeBaseObjectViewModel>();
            }
            else
            {
                return false;
            }
        }
    }
}
