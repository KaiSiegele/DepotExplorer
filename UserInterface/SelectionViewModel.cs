namespace UserInterface
{
    /// <summary>
    /// Das SelectionViewModel erlaubt es,
    /// Angaben fuer eine Abfrage einzugeben
    /// </summary>
    public class SelectionViewModel : BaseViewModel
    {
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Zeigt an, ob Daten ausgewählt werden können
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                UpdateProperty("Enabled");
            }
        }
        private bool _enabled = false;

        /// <summary>
        /// Zeigt an, ob das ViewModel seit dem Ausführen
        /// der letzten Abfrage aktualisert worden ist
        /// </summary>
        public bool Updated
        {
            get
            {
                return _updated;
            }
            set
            {
                _updated = value;
                OnPropertyChanged("Updated");
            }
        }
        private bool _updated = false;

        protected void UpdateProperty(string field)
        {
            Updated = true;
            OnPropertyChanged(field);
        }
    }
}
