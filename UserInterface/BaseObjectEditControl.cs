using Basics;
using Tools;
using System.Diagnostics;
using System.Windows.Controls;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für ein UserControl, wo alle Eingaben
    /// zu einem Objekt geändert oder zu einem neuen
    /// Objekt neu erfasst werden können
    /// </summary>
    public class BaseObjectEditControl : UserControl
    {
        public BaseObjectEditControl()
        {
            Modus = EditingModus.Nothing;
        }

        /// <summary>
        /// Prueft ob im Control gerade das Objekt mit der
        /// uebergebenen Id angezeigt wird. Falls ja, wird
        /// die Bearbeitung beendet
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        public void TryFinishViewingObject(int id)
        {
            if (Modus == EditingModus.Viewing)
            {
                if (ObjectId == id)
                {
                    FinishEditingObject(GetObject(), true);
                }
            }
        }

        /// <summary>
        /// Signatur fuer eine Callback-Methode, um zu erfahren,
        /// dass der Modus geändert wurde
        /// </summary>
        /// <param name="id">Id des Objekts, dass gerade bearbeitet wird</param>
        /// <param name="modus">Neuer Modus</param>
        public delegate void OnModeHasChanged(int id, EditingModus modus);

        public OnModeHasChanged ModeChanged;

        #region Methode die in der Basis-Klasse ueberschrieben werden koennen
        /// <summary>
        /// Gibt das Objekt zurück, das gerade verarbeitet wird
        /// </summary>
        /// <returns>Objekt, das gerade verarbeitet wird</returns>
        public virtual BaseObject GetObject()
        {
            return null;
        }

        /// <summary>
        /// Stellt fest, ob an den Eingaben Änderungen durchgeführt wurden
        /// </summary>
        /// <returns>Änderung wurde durchgeführt</returns>
        protected virtual bool HasObjectChanged()
        {
            return false;
        }

        /// <summary>
        /// Stellt fest, ob alle Eingaben korrekt sind
        /// </summary>
        /// <returns>Alles ist korrekt</returns>
        protected virtual bool IsObjectValid()
        {
            return false;
        }

        /// <summary>
        /// Aktualisiert das Control, um ein Objekt ändern  zu können
        /// Subcontrols werden bspw. aktivert, das Objekt wird an die View
        /// Models gebunden, Comboboxen mit Auswahllisten gefüllt
        /// </summary>
        /// <param name="bo">Zu änderndes Objekt</param>
        /// <param name="modus">Änderungsmodus</param>
        protected virtual void UpdateBeforeEditing(BaseObject bo, EditingModus modus)
        {
        }

        /// <summary>
        /// Aktualisiert das Control nachdem die Änderung abgeschlossen ist.
        /// Subcontrols werden wieder deaktiviert usw.
        /// </summary>
        protected virtual void UpdateAfterEditing()
        {
        }

        /// <summary>
        /// Aktualisiert das Control nachdem ein Objekt gespeichert wurde.
        /// </summary>
        protected virtual void UpdateAfterSaving()
        {
        }
        #endregion

        /// <summary>
        /// Zeigt an, ob auf der rechten Seite des Controls 
        /// ein Objekt editiert wird
        /// </summary>
        protected bool Editing
        {
            get
            {
                return Modus.IsEditing();
            }
        }

        /// <summary>
        /// Zeigt an, ob auf der rechten Seite des Controls 
        /// ein Objekt geändert oder Neu erfasst wird
        /// </summary>
        public bool Modifying
        {
            get
            {
                return Modus.IsModifying();
            }
        }

        /// <summary>
        /// Gibt den Modus in dem sich das Control gerade befindet zurück
        /// </summary>
        public EditingModus Modus { get; protected set; }

        /// <summary>
        /// Gibt die Id des Objekts zurück, dass gerade
        /// verarbeitet wird
        /// </summary>
        protected int ObjectId
        {
            get
            {
                BaseObject bo = GetObject();
                if (bo != null)
                {
                    return bo.Id;
                }
                else
                {
                    return BaseObject.NotSpecified;
                }
            }
        }

        /// <summary>
        /// Stellt fest, ob der Benutzer im Control
        /// den Save-Button drücken darf
        /// </summary>
        /// <returns>true wenn erlaubt, false sonst</returns>
        protected bool SaveObjectAllowed()
        {
            if (Modifying)
            {
                return (HasObjectChanged() && IsObjectValid());
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Starte die Bearbeitung eines Objekts
        /// </summary>
        /// <param name="bo">Zu bearbeitendes Objekt</param>
        /// <param name="modus">Edit-Modus</param>
        protected void StartEditingObject(BaseObject bo, EditingModus modus)
        {
            TraceInfo("StartEditingObject", "{0} {1}", modus, bo);
            CheckObject(bo, modus);
            UpdateModus(bo.Id, modus);
            UpdateBeforeEditing(bo, modus);
        }

        /// <summary>
        /// Speichert das übergebenen Objekt. Falls erfolgreich
        /// wird die Bearbeitung abgeschlossen.
        /// </summary>
        /// <param name="bdo">Zu speicherndes Objekt</param>
        /// <param name="close">true, wenn Bearbeitung endgueltig beendet werden soll, false sonst</param>
        protected void SaveObject(BaseDataObject bdo, bool close)
        {
            Debug.Assert(GetObject() == bdo.MainObject);
            bool result;
            if (bdo.MainObject.ObjectState == ObjectState.Inserted)
            {
                result = bdo.Insert();
            }
            else
            {
                result = bdo.Update();
            }

            if (result)
            {
                FinishEditingObject(GetObject(), close);
            }
        }
 
        /// <summary>
        /// Behandelt den Fall, dass der Benutzer in einem Control
        /// den Esc-Button drückt.
        /// </summary>
        /// <param name="title">Titel für die Abfrage</param>
        /// <param name="message">Nachricht was verändert wurde</param>
        protected void HandleCancelEditing(string title, string message)
        {
            bool cancel;
            if (HasObjectChanged())
            {
                cancel = Prompts.AskUser(title, "{0} {1}", message, Properties.Resource.CancelEditing);
            }
            else
            {
                cancel = true;
            }

            if (cancel)
            {
                FinishEditingObject(GetObject(), true);
            }
        }

        /// <summary>
        /// Prüft, ob das neu erzeugte Objekt eine gültige Id hat
        /// </summary>
        /// <param name="title">Titel des Dialogs</param>
        /// <param name="desc"></param>
        /// <param name="bo">Neues Objekt</param>
        /// <returns>Objekt hat eine gültige Id</returns>
        protected bool CheckNewObject(string title, string desc, BaseObject bo)
        {
            Debug.Assert(bo != null);
            bool result = true;
            if (bo.Id == BaseObject.NotSpecified)
            {
                string message = Properties.Resource.ResourceManager.GetMessageFromResource("ErrorWhenCreatingObject", desc);
                Prompts.ShowError(title, message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Beendet die Bearbeitung eines Objekts
        /// </summary>
        /// <param name="bo">Zu bearbeitendes Objekt</param>
        /// <param name="close">true, wenn Bearbeitung endgueltig beendet werden soll, false sonst</param>
        private void FinishEditingObject(BaseObject bo, bool close)
        {
            if (close)
            {
                TraceInfo("FinishEditingObject", "Beende {0} {1}", Modus, bo);
                UpdateModus(BaseObject.NotSpecified, EditingModus.Nothing);
                UpdateAfterEditing();
            }
            else
            {
                TraceInfo("FinishEditingObject", "{0} gespeichert", bo);
                var modus = Modifying ? EditingModus.Updating : EditingModus.Viewing;
                CheckObject(bo, modus);
                UpdateModus(bo.Id, modus);
                UpdateAfterSaving();
            }
        }


        /// <summary>, 
        /// Aktualisiert den Modus
        /// </summary>
        /// <param name="id">Id des Objekts, dass bearbeitet/angezeigt wird</param>
        /// <param name="modus">Neuer Modus</param>
        private void UpdateModus(int id, EditingModus modus)
        {
            Modus = modus;
            if (ModeChanged != null)
            {
                ModeChanged(id, modus);
            }
        }

        private void TraceInfo(string method, string message, params object[] args)
        {
            Log.Write(TraceLevel.Info, GetType().Name, method, message, args);
        }

        [Conditional("DEBUG")]
        private void CheckObject(BaseObject bo, EditingModus modus)
        {
            Debug.Assert(bo != null);
            switch (modus)
            {
                case EditingModus.Nothing:
                    break;
                case EditingModus.Inserting:
                    Debug.Assert(bo.ObjectState == ObjectState.Inserted);
                    BaseObject.CheckId(bo.Id);
                    break;
                case EditingModus.Updating:
                case EditingModus.Viewing:
                    Debug.Assert(bo.ObjectState == ObjectState.Stored);
                    BaseObject.CheckId(bo.Id);
                    break;
            }
        }
    }
}
