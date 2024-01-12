using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Basics
{
    public class DoubleRangeValidation : BaseRangeValidation
    {
        public DoubleRangeValidation(string fieldname, double minValue, bool includeMinimum, double maxValue, bool includeMaximum)
            : this(fieldname, minValue, true, includeMinimum, maxValue, true, includeMaximum)
        {
            Debug.Assert(minValue < maxValue);
        }

        public DoubleRangeValidation(string fieldname, double value, bool isMinValue, bool include)
            : this(fieldname, value, isMinValue == false, include, value, isMinValue == true, include)
        {
        }

        protected DoubleRangeValidation(string fieldname, double minValue, bool checkMinimum, bool includeMinimum, double maxValue, bool checkMaximum, bool includeMaximum)
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
                double value = System.Convert.ToDouble(obj);
                result = CheckInput<double>(value, _minValue, _maxValue, ref error);
            }
            catch (Exception ex)
            {
                ShowConvertError(obj, "double", ex);
                error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_Error", FieldName);
            }
            return result;
        }

        private readonly double _minValue;
        private readonly double _maxValue;
    }
}
