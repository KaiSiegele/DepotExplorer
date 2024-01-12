using Basics;
using System.Collections.Generic;
using System.Diagnostics;
using Tools;

namespace UserInterface
{
    /// <summary>
    /// Hilfsklasse, um zu speichern, in welchen EditControls gerade Objekte angezeigt oder bearbeitet werden.
    /// </summary>
    public static class UserEditings
    {
        /// <summary>
        /// Stellt fest, ob im Control mit der übergebenen Id gerade ein Object angezeigt oder verändert wird
        /// </summary>
        /// <param name="editControlId">Id des EditControls</param>
        /// <param name="mustModifying">Das Objekt muss verändert werden</param>
        /// <returns>true, wenn im Control ein Objekt verändert oder bearbeitet wird, false sonst</returns>
        public static bool HasUserEditing(int editControlId, bool mustModifying)
        {
            var userEditing = GetUserEditing(editControlId);
            if (userEditing != null)
            {
                return (mustModifying == false || userEditing.EditingModus.IsModifying());
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft, dass im Control mit der übergebenen Id gerade kein Object angezeigt oder bearbeitet wird
        /// </summary>
        /// <param name="editControlId">Id des EditControls</param>
        /// <param name="mustModifying">Das Objekt muss verändert werden</param>
        /// <param name="title">Titel für die angezeigte Warnung</param>
        /// <param name="message">Anzuzeigende Warnung</param>
        /// <returns>true, wenn im Control kein Objekt  verändert oder bearbeitet wird, false sonst</returns>
        public static bool CheckNoUserEditing(int editControlId, bool mustModifying, string title, string message)
        {
            bool result = true;
            if (HasUserEditing(editControlId, mustModifying))
            {
                result = false;
                Prompts.ShowInfo(title, message);
            }
            return result;
        }

        /// <summary>
        /// Fügt ein, dass im EditControl ein Objekt angezeigt oder bearbeitet wird.
        /// Precondition: Für das EditControl darf noch nicht erfasst worden sein, dass ein Objekt angezeigt oder bearbeitet wird.
        /// </summary>
        /// <param name="editControlId">Id des EditControls</param>
        /// <param name="editingControl">EditControl</param>
        /// <param name="editingModus">Editiermodus</param>
        /// <param name="editingObject">Objekt, dass angezeigt oder bearbeitet wird</param>
        public static void InsertUserEditing(int editControlId, BaseObjectEditControl editingControl, EditingModus editingModus, BaseObject editingObject)
        {
            CheckExistence(editControlId, false);
            var newUserEditing = new UserEditing(editingControl, editingModus, editingObject);
            dictUserEditings.Add(editControlId, newUserEditing);
        }

        /// <summary>
        /// Löscht, dass im EditControl ein Objekt angezeigt oder bearbeitet wird.
        /// Precondition: Für das EditControl muss erfasst worden sein, dass ein Objekt angezeigt oder bearbeitet wird.
        /// </summary>
        /// <param name="editControlId">Id des EditControls</param>
        public static void RemoveUserEditing(int editControlId)
        {
            CheckExistence(editControlId, true);
            dictUserEditings.Remove(editControlId);
        }

        private static UserEditing GetUserEditing(int editControlId)
        {
            UserEditing userEditing = null;
            if (dictUserEditings.ContainsKey(editControlId))
            {
                userEditing = dictUserEditings[editControlId];
            }
            return userEditing;
        }

        [Conditional("DEBUG")]
        static void CheckExistence(int editControlId, bool mustExists)
        {
            if (HasUserEditing(editControlId, false))
            {
                Debug.Assert(mustExists == true, "Eintrag muss vorhanden sein");
            }
            else
            {
                Debug.Assert(mustExists == false, "Eintrag darf nicht vorhanden sein");
            }
        }

        private class UserEditing
        {
            public UserEditing(BaseObjectEditControl editingControl, EditingModus editingModus, BaseObject editingObject)
            {
                EditingControl = editingControl;
                EditingModus = editingModus;
                EditingObject = editingObject;
            }

            public override string ToString()
            {
                return string.Format("UserEditing(Modus: {0}, Object: {1})", EditingModus, EditingObject);
            }

            public BaseObjectEditControl EditingControl { get; private set; }

            public EditingModus EditingModus { get; private set; }

            public BaseObject EditingObject { get; private set; }
        }
       
        private static readonly string ownClassName = "UserEditings";

        static readonly Dictionary<int, UserEditing> dictUserEditings = new Dictionary<int, UserEditing>();
    }
}
