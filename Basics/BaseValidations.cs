using System.Collections.Generic;
using System.Diagnostics;

namespace Basics
{
    /// <summary>
    /// Dictionary, um fuer alle Objekte des Programms die Validations
    /// zu speichern (->BaseValidation). Innerhalb des Dictionaries
    /// werden die Validations ueber den Typ des Objekts und den Feldnamen
    /// angesprochen
    /// </summary>
    public static class BaseValidations
    {
        /// <summary>
        /// Construktor
        /// </summary>
        static BaseValidations()
        {
        }

        /// <summary>
        /// Fuegt eine neue Validation fuer einen von
        /// BaseObject abgeleiteten Objekt-Typ ein
        /// </summary>
        /// <typeparam name="T">Validation</typeparam>
        /// <param name="bv">Objekt-Typ</param>
        public static void AddValidation<T>(BaseValidation bv) where T : BaseObject
        {
            AddValidation(CreateName(typeof(T).Name, bv.FieldName), bv);
        }

        /// <summary>
        /// Ueberprueft, ob der Inhalt des Felds gueltig ist
        /// </summary>
        /// <param name="fieldname">Name des Felds</param>
        /// <param name="input">Inhalt</param>
        /// <param name="bo">Objekt fuer die Ueberpruefung</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>true, wenn Inhalt ok, false sonst</returns>
        public static bool ValidateInput(string fieldname, object input, BaseObject bo, ref string error)
        {
            return ValidatingInput(CreateName(bo.GetType().Name, fieldname), input, bo, ref error);
        }

        private static void AddValidation(string name, BaseValidation bv)
        {
            if (!dictValidation.ContainsKey(name))
            {
                dictValidation.Add(name, bv);
            }
            else
            {
                Debug.Assert(false, "Validation fuer Name " + name + " bereits definiert");
            }
        }

        static bool ValidatingInput(string name, object input, BaseObject bo, ref string error)
        {
            BaseValidation bv;
            if (dictValidation.TryGetValue(name, out bv))
            {
                return bv.ValidateInput(input, bo, ref error);
            }
            else
            {
                return true;
            }
        }

        static string CreateName(string typename, string fieldname)
        {
            return string.Format("{0}.{1}", typename, fieldname);
        }

        static readonly Dictionary<string, BaseValidation> dictValidation = new Dictionary<string, BaseValidation>();
    }
}
