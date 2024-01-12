using System.ComponentModel;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse fuer ViewModels
    /// Das ViewModel (Publisher) erlaubt es Interessierte
    /// (Subscriber) sich anzumelden und über Änderungen 
    /// informiert zu werden
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /// <summary>
        /// Zeigt (der View) an, dass die Eigenshaft mit dem Namen verändert wurde
        /// </summary>
        /// <param name="propertyName">Name der Eigneschaft</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
