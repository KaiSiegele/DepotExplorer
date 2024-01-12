using System;
using System.Diagnostics;
using Tools;

namespace Basics
{
    /// <summary>
    /// Basisklasse, um den Inhalt eines Feldes eines Objekts zu ueberpruefen
    /// </summary>
    public abstract class BaseValidation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldname">Feldname</param>
        protected BaseValidation(string fieldname)
        {
            FieldName = fieldname;
        }

        /// <summary>
        /// Prueft ob die Eingabe fuer das Feld korrekt ist
        /// </summary>
        /// <param name="obj">Eingabe</param>
        /// <param name="bo">Zu aendernders Objekt</param>
        /// <param name="error">Fehler, falls Eingabe falsch</param>
        /// <returns>Eingabe korrekt</returns>
        public abstract bool ValidateInput(object obj, BaseObject bo, ref string error);

        public string FieldName { get; private set; }

        /// <summary>
        /// Zeigt einen Fehler an, dass sich die Eingabe nicht
        /// in den gewuenschten Typ umwandeln lies und eine Exception
        /// ausgeloest wurde
        /// </summary>
        /// <param name="obj">Eingabe</param>
        /// <param name="typeName">Typ, in dem die Eingabe konvertiert werden soll</param>
        /// <param name="ex">Exception</param>
        protected void ShowConvertError(object obj, string typeName, Exception ex)
        {
            string message = string.Format("Fehler bei der Konvertierung von {0} nach {1}: {2}", obj, typeName, ex.Message);
            Log.Write(TraceLevel.Error, "Validation", "ValidateInput", message);
            Debug.Assert(false, message);
        }
    }

    /// <summary>
    /// Klasse fuer die Ueberpruefung, dass der Wert des
    /// Feldes eine Enumeration ungleich dem Default-Typ ist.
    /// Der Default-Typ wird bei bei den Enumerationen als
    /// leere Auswahl verwendet.
    /// </summary>
    /// <typeparam name="T">Enumerations-Typ</typeparam>
    public class EnumValidation<T> : BaseValidation where T : struct
    {
        public EnumValidation(string fieldname)
          : base(fieldname)
        {
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            bool result = false;
            try
            {
                T value;
                if (Enum.TryParse<T>(obj.ToString(), out value))
                {
                    if (value.Equals(default(T)))
                    {
                        error = Properties.Resource.ResourceManager.GetMessageFromResource("EnumValidation_NoSelection", FieldName);
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    error = Properties.Resource.ResourceManager.GetMessageFromResource("EnumValidation_Error", FieldName);
                }
            }
            catch (Exception ex)
            {
                ShowConvertError(obj, "Enumeration", ex);
                error = Properties.Resource.ResourceManager.GetMessageFromResource("EnumValidation_Error", FieldName);
            }
            return result;
        }
    }

    /// <summary>
    /// Klasse fuer die Ueberpruefung, dass der Wert des Feldes 
    /// eine Id ist, die auf ein anderes Objekt verweist.
    /// </summary>
    public class LinkValidation : BaseValidation
    {
        public LinkValidation(string fieldname)
          : base(fieldname)
        {
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            bool result = false;
            try
            {
                int value = System.Convert.ToInt32(obj);
                if (value == BaseObject.NotSpecified)
                {
                    error = Properties.Resource.ResourceManager.GetMessageFromResource("LinkValidation_NotSpecified", FieldName);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ShowConvertError(obj, "Id", ex);
                error = Properties.Resource.ResourceManager.GetMessageFromResource("LinkValidation_Error", FieldName);
            }
            return result;
        }
    }
}