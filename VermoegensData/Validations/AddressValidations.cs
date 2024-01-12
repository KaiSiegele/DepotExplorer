using Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Ueberprüft für eine Addresse, dass die eingegebene 
    /// Postleitzahl das richtige Format hat
    /// </summary>
    internal class PLZValidation : StringValidation
    {
        public PLZValidation()
            : base("PLZ", 5)
        {
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            return ValidatePLZ(obj.ToString(), ref error);
        }

        private bool ValidatePLZ(string input, ref string error)
        {
            bool result = false;
            if (base.ValidateInput(input, ref error))
            {
                if (input.AreAllDigits())
                {
                    result = true;
                }
                else
                {
                    error = Properties.Resources.PLZValidation_WrongFormat;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Ueberprüft für eine Addresse, dass der eingegebene 
    /// Ort das richtige Format hat
    /// </summary>
    internal class OrtValidation : StringValidation
    {
        public OrtValidation()
            : base("Ort", 2, 30)
        {

        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            return ValidateOrt(obj.ToString(), ref error);
        }

        private bool ValidateOrt(string input, ref string error)
        {
            bool result = false;
            if (base.ValidateInput(input, ref error))
            {
                if (input.IsLocationName())
                {
                    result = true;
                }
                else
                {
                    error = Properties.Resources.OrtValidation_WrongFormat;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Ueberprüft für eine Addresse, dass das eingegebene 
    /// Land das richtige Format hat
    /// </summary>
    internal class LandValidation : StringValidation
    {
        public LandValidation()
            : base("Land", 3, 30)
        {
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            return ValidateOrt(obj.ToString(), ref error);
        }

        private bool ValidateOrt(string input, ref string error)
        {
            bool result = false;
            if (base.ValidateInput(input, ref error))
            {
                if (input.IsLocationName()  || input.IsAbbreviation())
                {
                    result = true;
                }
                else
                {
                    error = Properties.Resources.LandValidation_WrongFormat;
                }
            }
            return result;
        }
    }
}
