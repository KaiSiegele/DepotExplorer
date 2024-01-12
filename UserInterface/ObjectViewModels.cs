using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Basics;
using Tools;
using System.Diagnostics;

namespace UserInterface
{
    /// <summary>
    /// Liste von ObjectViewModel, um die Daten 
    /// mehrerer Modelle (Objekte der Klasse BaseObject)
    /// anzuzeigen oder zu verändern.
    /// </summary>
    public abstract class ObjectViewModels : ObservableCollection<ObjectViewModel>, IObjectEditComponent
    {
        /// <summary>
        /// Constructor
        /// Aufzurufen von der abgeleiteten Klasse
        /// </summary>
        /// <param name="bos">Anzueigende und zu verändernde Objekte</param>
        protected ObjectViewModels(BaseObjects bos)
          : this(bos, "Name", true)
        {
        }
        /// <summary>
        /// Constructor
        /// Aufzurufen von der abgeleiteten Klasse
        /// </summary>
        /// <param name="sortCriterium">Sortierungskriterium</param>
        /// <param name="bos">Anzueigende und zu verändernde Objekte</param>
        protected ObjectViewModels(BaseObjects bos, string sortCriterium)
          : this(bos, sortCriterium, true)
        {
        }

        /// <summary>
        /// Constructor
        /// Aufzurufen von der abgeleiteten Klasse
        /// </summary>
        /// <param name="bos">Anzuzuzeigenden und zu verändernde BaseObject-Objekte</param>
        /// <param name="sortCriterium">Sortierungskriterium</param>
        /// <param name="isEmptyValid">Objekt ist gültig, wenn keine Objekte angezeigt werden</param>
        protected ObjectViewModels(BaseObjects bos, string sortCriterium, bool isEmptyValid)
        {
            _bos = bos;
            _changed = false;
            _isEmptyValid = isEmptyValid;

            _comparer = new ObjectViewModelComparer(sortCriterium);

            ConnectEventhandler();
        }

        ~ObjectViewModels()
        {
            DisconnectEventhandler();
        }

        /// <summary>
        /// Erzeugt zum Objekt ein ViewModel, um die Daten
        /// anzuzeigen und zu verändern
        /// </summary>
        /// <param name="baseObject">Objekt</param>
        /// <returns>Objekt, von einer von BaseObjectViewModel abgeleiteten Klasse</returns>
        protected abstract ObjectViewModel CreateViewModel(BaseObject baseObject);

        /// <summary>
        /// Stellt fest, ob für das übergebene Objekt ein ViewModel erzeugt werden
        /// soll und in die Liste eingefügt werden soll
        /// </summary>
        /// <param name="baseObject">Zu prüfendes Objekt</param>
        /// <returns>true, wenn ein ViewModel eingefügt werden soll</returns>
        protected virtual bool ShouldInsertViewModel(BaseObject baseObject)
        {
            return true;
        }

        /// <summary>
        /// Gibt das ObjektViewModel des Objekts mit der übergebenen Id zurück
        /// </summary> 
        /// <param name="id">Id des Objekts</param>
        /// <returns>Gefundenes ObjectViewModel oder null falls nicht vorhanden</returns>
        public ObjectViewModel GetObjectViewModel(int id)
        {
            return (from bovm in this where bovm.BaseObject.Id == id select bovm).FirstOrDefault();
        }

        /// <summary>
        /// Gibt das von ObjektViewModel abgeleitete ViewModel 
        /// des Objekts mit der übergebenen Id zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ von ObjectViewModel</typeparam>
        /// <param name="id">Id des Objekts</param>
        /// <returns>Gefundenes ViewModel oder null falls nicht vorhanden></returns>
        public T GetViewModel<T>(int id) where T : ObjectViewModel
        {
            var ovm = GetObjectViewModel(id);
            if (ovm != null)
            {
                return ovm as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Löscht das ViewModel des Objekts mit der übergebenen Id
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        public void RemoveViewModel(int id)
        {
            OnObjectRemoved(id);
        }

        /// <summary>
        /// Fügt das ViewModel des Objekts mit der übergebenen Id in die Liste ein
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        public void InsertViewModel(int id)
        {
            ObjectViewModel bvm = GetObjectViewModel(id);
            if (bvm == null)
            {
                BaseObject bo = _bos[id];
                if (bo != null)
                {
                    OnObjectInserted(id, bo);
                }
                else
                {
                    Log.Write(TraceLevel.Error, GetType().Name, "InsertViewModel", "Object mit der {0} nicht gefunden", id);
                }
            }
            else
            {
                Log.Write(TraceLevel.Error, GetType().Name, "InsertViewModel", "{0} mit der {1} bereits eingefügt ", bvm.BaseObject, id);
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der ViewModels
        /// indem sie aus der Objetliste neu geladen werden
        /// </summary>
        public void RefreshViewModels()
        {
            Clear();
            LoadViewModels();
        }

        /// <summary>
        /// Sortiert die ViewModels nach der übergebenen Kriterium
        /// Sollte sich das Kriterium seit dem letzten Aufruf nicht geändert
        /// haben, ändert sich die Sortierungsreihenfolge
        /// </summary>
        /// <param name="sortCriterium">Sortierungskriterium</param>
        public void SortViewModels(string sortCriterium)
        {
            var lstViewModels = new List<ObjectViewModel>(this);

            _comparer.UpdateSortCriterium(sortCriterium);
            lstViewModels.Sort(_comparer);

            Clear();
            AddViewModels(lstViewModels);
        }

        /// <summary>
        /// Setzt bei allen ViewModels zurück, dass
        /// sie geändert wurden.
        /// </summary>
        public void ResetViewModels()
        {
            _changed = false;
            foreach (var bovm in Items)
            {
                if (bovm.HasChanged())
                {
                    EditObjectViewModel ebovm = bovm as EditObjectViewModel;
                    if (ebovm != null)
                    {
                        ebovm.Reset();
                    }
                }
            }
        }

        #region Implementierung der Schnitstelle IObjectEditComponent
        public bool Valid
        {
            get
            {
                bool result;
                if (Count == 0)
                {
                    result = _isEmptyValid;
                }
                else
                {
                    result = Items.All(bovm => bovm.IsValid());
                }
                return result;
            }
        }

        public bool Changed
        {
            get
            {
                return _changed || Items.Any(bovm => bovm.HasChanged());
            }
        }
        #endregion


        #region Eventhandler, die auf Ereignisse der Liste von Objekten reagieren
        private void OnObjectInserted(int id, BaseObject baseObject)
        {
            if (ShouldInsertViewModel(baseObject))
            {
                _changed = true;
                Add(CreateViewModel(baseObject));
            }
        }

        private void OnObjectUpdated(int id, BaseObject baseObject)
        {
            ObjectViewModel bvm = GetObjectViewModel(id);
            if (bvm != null)
            {
                _changed = true;
                bvm.Update(baseObject);
            }
        }

        private void OnObjectRemoved(int id)
        {
            ObjectViewModel bvm = GetObjectViewModel(id);
            if (bvm != null)
            {
                _changed = true;
                Remove(bvm);
            }
        }

        private void OnObjectsCleared()
        {
            Clear();
            _changed = false;
        }

        private void OnObjectsLoading(BaseObject bo)
        {
            if (Count > 0)
            {
                Clear();
            }
        }

        private void OnObjectsLoaded(BaseObject bo)
        {
            _changed = false;
            LoadViewModels();
        }
        #endregion

        private void LoadViewModels()
        {
            var lstViewModels = new List<ObjectViewModel>(this);

            foreach (KeyValuePair<int, BaseObject> kvp in _bos)
            {
                if (ShouldInsertViewModel(kvp.Value))
                {
                    lstViewModels.Add(CreateViewModel(kvp.Value));
                }
            }
            AddViewModels(lstViewModels);
        }

        private void AddViewModels(IList<ObjectViewModel> lstBvms)
        {
            foreach (ObjectViewModel bvm in lstBvms)
            {
                Add(bvm);
            }
        }

        private void ConnectEventhandler()
        {
            _bos.Inserted += OnObjectInserted;
            _bos.Updated += OnObjectUpdated;
            _bos.Removed += OnObjectRemoved;
            _bos.Loading += OnObjectsLoading;
            _bos.Loaded += OnObjectsLoaded;
            _bos.Cleared += OnObjectsCleared;
        }

        private void DisconnectEventhandler()
        {
            _bos.Inserted -= OnObjectInserted;
            _bos.Updated -= OnObjectUpdated;
            _bos.Removed -= OnObjectRemoved;
            _bos.Loading -= OnObjectsLoading;
            _bos.Loaded -= OnObjectsLoaded;
            _bos.Cleared -= OnObjectsCleared;
        }

        private readonly ObjectViewModelComparer _comparer = null;
        private BaseObjects _bos = null;
        private bool _changed;
        private bool _isEmptyValid;
    }
}
