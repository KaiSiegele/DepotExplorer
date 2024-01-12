using Basics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace UserInterface
{
 
    public static class UserEditings
    {
        public class UserEditing
        {
            public UserEditing(BaseEditControl editingControl, EditingModus editingModus, BaseObject editingObject)
            {
                EditingControl = editingControl;
                EditingModus = editingModus;
                EditingObject = editingObject;
            }
            public override string ToString()
            {
                return string.Format("UserEditing(Control: {0}, Modus: {1}, Object: {2}",
                    EditingControl.GetType().Name, EditingModus, EditingObject);
            }


            public BaseEditControl EditingControl { get; private set; }

            public EditingModus EditingModus { get; private set; }

            public BaseObject EditingObject { get; private set; }
        }

        public static UserEditing GetUserEditing(int editControlId)
        {
            UserEditing userEditing = null;
            if (dictUserEditings.ContainsKey(editControlId))
            {
                userEditing = dictUserEditings[editControlId];
            }
            Log.Write(TraceLevel.Verbose, ownClassName, "GetUserEditing", "controlId {0} => {1}", editControlId, userEditing == null ? "null", userEditing);
            return userEditing;
        }
        public static void InsertUserEditing(int editControlId, BaseEditControl editingControl, EditingModus Editing, BaseObject editingObject)
        {
            CheckExistence(editControlId, false);
            var newUserEditing = new UserEditing(editingControl, Editing, editingObject);
            Log.Write(TraceLevel.Verbose, ownClassName, "InsertUserEditing", "controlId {0}, {1}", editControlId, newUserEditing);
            dictUserEditings.Add(editControlId, newUserEditing);
        }

        public static void RemoveUserEditing(int editControlId)
        {
            CheckExistence(editControlId, true);
            Log.Write(TraceLevel.Verbose, ownClassName, "RemoveUserEditing", "controlId {0}", editControlId);
            dictUserEditings.Remove(editControlId);
        }

        public static EditingModus GetUserEditingModus(int editControlId)
        {
            var userEditing = GetUserEditing(editControlId);
            if (userEditing == null)
            {
                return userEditing.EditingModus;
            }
            else
            {
                return EditingModus.Nothing;
            }

        }

        public static bool HasUserEditing(int editControlId)
        {
            return GetUserEditing(editControlId) != null;
        }




        [Conditional("DEBUG")]
        static void CheckExistence(int editControlId, bool mustExists)
        {
            if (dictUserEditings.ContainsKey(editControlId))
            {
                Debug.Assert(mustExists == true, "Eintrag muss vorhanden sein");
            }
            else
            {
                Debug.Assert(mustExists == false, "Eintrag darf nicht vorhanden sein");
            }
        }

        private static readonly string ownClassName = "UserEditings";

        static readonly Dictionary<int, UserEditing> dictUserEditings = new Dictionary<int, UserEditing>();
    }
}
