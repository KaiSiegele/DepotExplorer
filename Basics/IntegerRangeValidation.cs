using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Basics
{
    public class IntegerRangeValidation : BaseRangeValidation
    {
        /// <summary>
        /// Construktor fuer die geschlossenen Intervallle
        /// </summary>
        /// <param name="fieldname">Feldname</param>
        /// <param name="minValue">Minimaler Wert</param>
        /// <param name="includeMinimum">true, wenn minimaler Wert Teil des Intervalle ist, false sonst</param>
        /// <param name="maxValue">Maximaler Wert</param>
        /// <param name="includeMaximum">true, wenn maximaler Wert Teil des Intervalle ist, false sonst</param>
        public IntegerRangeValidation(string fieldname, int minValue, bool includeMinimum, int maxValue, bool includeMaximum)
            : this(fieldname, minValue, true, includeMinimum, maxValue, true, includeMaximum)
        {
            Debug.Assert(minValue < maxValue);
        }

        /// <summary>
        /// Construktor fuer die offenen Intervallle
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="value">Wert</param>
        /// <param name="isMinValue">true, wenn der Wert der minimale Wert ist, false wenn der Wert der maximale Wert ist</param>
        /// <param name="include">true, wenn der Wert Teil des Intervalle ist, false sonst</param>
        public IntegerRangeValidation(string fieldname, int value, bool isMinValue, bool include)
            : this(fieldname, value, isMinValue == true, include, value, isMinValue == false, include)
        {
        }
        protected IntegerRangeValidation(string fieldname, int minValue, bool checkMinimum, bool includeMinimum, int maxValue, bool checkMaximum, bool includeMaximum) 
            : base(fieldname, checkMinimum, includeMinimum, checkMaximum, includeMaximum)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            bool result = false;
            try
            {
                int value = System.Convert.ToInt32(obj);
                result = CheckInput<int>(value, _minValue, _maxValue, ref error);
            }
            catch (Exception ex)
            {
                ShowConvertError(obj, "Integer", ex);
                error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_Error", FieldName);
            }
            return result;
        }

        private readonly int _minValue;
        private readonly int _maxValue;
    }
}
