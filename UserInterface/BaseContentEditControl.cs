using Basics;
using System.Diagnostics;
using System.Windows.Controls;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für ein UserControl, wo Teile
    /// der Eingaben zu einem Objekt geändert 
    /// oder zu einem neuen Objekt neu erfasst 
    /// werden können
    /// Objekte dieser Klasse werden innerhalb der Objekte
    /// der Klasse BaseEditControl verwendet
    /// </summary>
    public class BaseContentEditControl : UserControl
    {
        public BaseContentEditControl()
        {
            ObjectId = BaseObject.NotSpecified;
            Editing = false;
            IsReadOnly = false;
        }

        /// <summary>
        /// Startet die Bearbeitung des Inhalts
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        /// <param name="isReadOnly">Zeigt an, dass der Inhalt nur angesehen werden kann</param>
        public void StartEditingContent(int id, bool isReadOnly = false)
        {
            Check(false);
            ObjectId = id;
            Editing = true;
            IsReadOnly = isReadOnly;

            UpdateBeforeEditing(IsReadOnly);
        }

        /// <summary>
        /// Beendet die Bearbeitung des Objekts
        /// </summary>
        /// <param name="close">true, wenn Bearbeitung endgueltig beendet werden soll, false sonst</param>
        public void FinishEditingContent(bool close)
        {
            Check(true);
            if (close)
            {
                UpdateAfterEditing(IsReadOnly);

                ObjectId = BaseObject.NotSpecified;
                Editing = false;
                IsReadOnly = false;
            }
            else
            {
                UpdateAfterSaving();
            }
        }

        public int ObjectId { get; private set; }
        
        /// <summary>
        /// Zeit an, ob der Inhalt des Controls geaendert werden darf
        /// </summary>
        /// <returns>true wenn Inhalt geaendert werden darf, false sonst</returns>
        protected bool EditContentAllowed()
        {
            return Editing && (IsReadOnly == false);
        }

        #region Schnittstelle
        /// <summary>
        /// Aktualisiert das Control, um den Inhalt
        /// eines Objekt ändern zu können
        /// </summary>
        /// <param name="isReadOnly">Zeigt an, dass der Inhalt nur angesehen werden kann</param>
        protected virtual void UpdateBeforeEditing(bool isReadOnly)
        {
        }

        /// <summary>
        /// Aktualisiert das Control nachdem die Änderung abgeschlossen ist.
        /// </summary>
        /// <param name="isReadOnly">Zeigt an, dass der Inhalt nur angesehen werden kann</param>
        protected virtual void UpdateAfterEditing(bool isReadOnly)
        {
        }

        /// <summary>
        /// Aktualisiert das Control nachdem der Inhalt gespeichert wurde.
        /// </summary>
        protected virtual void UpdateAfterSaving()
        {
        }
        #endregion

        [Conditional("DEBUG")]
        private void Check(bool editing)
        {
            Debug.Assert(editing == Editing);
            Debug.Assert(editing != (ObjectId == BaseObject.NotSpecified));
        }

        private bool Editing { get; set; }

        private bool IsReadOnly { get; set; }
    }
}
