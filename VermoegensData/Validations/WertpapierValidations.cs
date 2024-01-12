using System;

using Basics;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Ueberprueft, dass alle Wertpapiere einen eindeutigen Namen haben
    /// </summary>
    internal class WertpapierNameValidation : StringValidation
    {
        public WertpapierNameValidation()
          : base("Name", 4, 20)
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
                Wertpapier wp = WertpapierInfo.GetWertpapierByName(input);
                result = ValidationTools.CheckValueUnique(bo, wp, FieldName, ref error);
            }
            return result;
        }
    }

    /// <summary>
    /// Ueberprueft, dass alle Wertpapiere eine eindeutign WKN haben,
    /// die ein korrektes Format hat
    /// </summary>
    internal class WKNValidation : StringValidation
    {
        public WKNValidation()
          : base("WKN", 6)
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
                Wertpapier wp = WertpapierInfo.GetWertpapierByWKN(input);
                result = ValidationTools.CheckValueUnique(bo, wp, FieldName, ref error);
            }
            else
            {
                error = Properties.Resources.WKNValidation_WrongFormat;
            }
            return result;
        }
    }

    /// <summary>
    /// Ueberprueft, dass alle Wertpapiere eine eindeutign ISIN haben,
    /// die ein korrektes Format hat
    /// </summary>
    internal class ISINValidation : StringValidation
    {
        public ISINValidation()
          : base("ISIN", 12)
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
                    Wertpapier wp = WertpapierInfo.GetWertpapierByISIN(input);
                    result = ValidationTools.CheckValueUnique(bo, wp, FieldName, ref error);
                }
                else
                {
                    error = Properties.Resources.ISINValidation_WrongFormat;
                }
            }
            return result;
        }
    }
}
