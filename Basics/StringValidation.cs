using Tools;

namespace Basics
{
    /// <summary>
    /// Basisklasse, um die Laenge einer Eingabe zu ueberpruefen 
    /// </summary>
    public class StringValidation : BaseValidation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldname">Feldname</param>
        /// <param name="minLength">Mindestlaenge</param>
        /// <param name="maxLength">Maximale Laenge</param>
        public StringValidation(string fieldname, int minLength, int maxLength)
          : base(fieldname)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldname">Feldname</param>
        /// <param name="length">Vorgegebene Laenge</param>
        public StringValidation(string fieldname, int length)
          : this(fieldname, length, length)
        {
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            return ValidateInput(obj.ToString(), ref error);
        }

        protected bool ValidateInput(string input, ref string error)
        {
            bool result;
            int length = string.IsNullOrWhiteSpace(input) ? 0 : input.Length;
            if (length == 0 && maxLength == 0 && minLength == 0)
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("StringValidation_NoInput", FieldName);
            }
            else if (maxLength == minLength && length != minLength)
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("StringValidation_WrongLength", FieldName, minLength);
            }
            else if (length < minLength)
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("StringValidation_InputTooShort", FieldName, minLength);
            }
            else if (length > maxLength)
            {
                result = false;
                error = Properties.Resource.ResourceManager.GetMessageFromResource("StringValidation_InputTooLong", FieldName, maxLength);
            }
            else
            {
                result = true;
            }
            return result;
        }

        private readonly int minLength;
        private readonly int maxLength;
    }

    /// <summary>
    /// Klasse, die ueberprueft, dass alle Objekte in der
    /// Liste einen eindeutigen Namen haben
    /// </summary>
    public class UniqueNameValidation : StringValidation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldname">Feldname</param>
        /// <param name="minLength">Mindestlaenge</param>
        /// <param name="maxLength">Maximale Laenge</param>
        /// <param name="bos">Liste, wo der Name der Objekte eindeutig sein muss</param>
        public UniqueNameValidation(string fieldname, int minLength, int maxLength, BaseObjects bos)
          : base(fieldname, minLength, maxLength)
        {
            _bos = bos;
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            return ValidateInput(obj.ToString(), bo, ref error);
        }

        private bool ValidateInput(string input, BaseObject bo, ref string error)
        {
            bool result = false;
            if (base.ValidateInput(input, ref error))
            {
                BaseObject boFound = _bos[input];
                result = ValidationTools.CheckNameUnique(bo, boFound, ref error);
            }
            return result;
        }

        private readonly BaseObjects _bos;
    }
}