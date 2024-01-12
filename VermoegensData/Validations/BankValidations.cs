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
    /// Ueberprüft für eine Bank, dass die eingegebene 
    /// Bankleitzah das richtige Format hat
    /// </summary>
    internal class BLZValidation : StringValidation
    {
        public BLZValidation()
            : base("Blz", 5, 8)
        {
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
                if (input.IsIdentificationCode())
                {
                    result = true;
                    Bank bankFound = CachedData.Banken.GetBankByBlz(input);
                    if (bankFound != null)
                    {
                        if (bo.Id != bankFound.Id)
                        {
                            result = false;
                            error = Properties.Resources.BlzValidation_NotUnique;
                        }
                    }
                }
                else
                {
                    error = Properties.Resources.BlzValidation_WrongFormat;
                }
            }
            return result;
        }
    }
}
