using System.Collections.Generic;
using Basics;
using System.Reflection;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für ObjectViewmodels, die die Daten eines
    /// Modells nur anzeigen aber nicht verändern kann
    /// </summary>
    public class ViewObjectViewModel : ObjectViewModel
    {
        /// <summary>
        /// Erzeugt ein neues Viewmodel
        /// </summary>
        /// <param name="bo">Modell für die Datenanzeige</param>
        protected ViewObjectViewModel(BaseObject bo) : base(bo)
        {
        }

        #region Methoden der Basisklasse
        public override bool HasChanged()
        {
            return false;
        }

        public override bool IsValid()
        {
            return _bo != null;
        }

        protected override void Refresh()
        {
            IEnumerable<PropertyInfo> props = GetType().GetProperties();
            foreach (var pi in props)
            {
                OnPropertyChanged(pi.Name);
            }
        }
        #endregion
    }
}
