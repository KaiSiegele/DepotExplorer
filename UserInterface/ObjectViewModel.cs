using System;
using System.Text;
using System.ComponentModel;
using Basics;

namespace UserInterface
{
    /// <summary>
    /// Das ObjectViewModel erlaubt es die Daten 
    /// eines Modells (Objekt der Klasse BaseObject)
    /// anzuzeigen und zu verändern
    /// </summary>
    public abstract class ObjectViewModel : BaseViewModel
    {
        /// <summary>
        /// Erzeugt ein neues ViewModel
        /// </summary>
        protected ObjectViewModel()
        {
            _bo = null;
        }

        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="bo">Modell, dessen Daten gelesen und verändert werden</param>
        protected ObjectViewModel(BaseObject bo)
        {
            Update(bo);
        }

        public override string ToString()
        {

            return string.Format("{0} ({1})", GetType().Name, _bo == null ? "null" : _bo.ToString());
        }

        /// <summary>
        /// Aktualisiert das Modell. Nachdem ein anderes Modell
        /// gesetzt wurde, müssen auch alle Eigenschaften des
        /// Viewmodells aktualisiert werden
        /// </summary>
        /// <param name="bo">Modell dessen Daten gelesen und verändert werden</param>
        /// <param name="enabled">Zeigt an ob die Daten des Modells geändert werden können</param>
        public void Update(BaseObject bo)
        {
            _bo = bo;

            Refresh();
        }

        /// <summary>
        /// Gibt das zu bearbeitende Modell zurück
        /// </summary>
        public BaseObject BaseObject
        {
            get { return _bo; }
        }

        /// <summary>
        /// Gibt die Id des zu bearbeitenden Modells zurück
        /// oder BaseObject.NotSpecified falls kein Modell
        /// gesetzt war
        /// </summary>
        public int ObjectId
        {
            get
            {
                if (BaseObject == null)
                    return BaseObject.NotSpecified;
                else
                    return BaseObject.Id;
            }
        }

        /// <summary>
        /// Wird aufgerufen nachdem ein neues Modell gesetzt wurde
        /// In diesem müssen alle Eigenschaften neu gesetzt werden
        /// und diese Änderungen angezeigt werden
        /// </summary>
        protected abstract void Refresh();

        /// <summary>
        /// Stellt fest, ob alle Angaben des ViewModels
        /// korrekt sind
        /// </summary>
        /// <returns>Alle Angaben sind korrekt</returns>
        public abstract bool IsValid();

        /// <summary>
        /// Stellt fest, ob Angaben des ViewModels verändert wurden
        /// </summary>
        /// <returns>Angaben wurden verändert</returns>
        public abstract bool HasChanged();


        protected BaseObject _bo;
    }


    /// <summary>
    /// Attribut, das anzeigt, ob die Eigenschaft des ViewModels auch
    /// eine Eigenschaft im Model (BaseObject) ist
    /// </summary>
    public class BaseDataProperty : Attribute
    {
        public BaseDataProperty()
        {
        }
    }
}
