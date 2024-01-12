using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Basics
{
    public class DateRangeValidation : BaseRangeValidation
    {
        public DateRangeValidation(string fieldname, DateTime minValue, bool includeMinimum, DateTime maxValue, bool includeMaximum)
            : this(fieldname, minValue, true, includeMinimum, maxValue, true, includeMaximum)
        {
            Debug.Assert(minValue < maxValue);
        }

        public DateRangeValidation(string fieldname, DateTime value, bool isMinValue, bool include)
            : this(fieldname, value, isMinValue == false, include, value, isMinValue == true, include)
        {
        }
        protected DateRangeValidation(string fieldname, DateTime minValue, bool checkMinimum, bool includeMinimum, DateTime maxValue, bool checkMaximum, bool includeMaximum)
            : base(fieldname, checkMinimum, includeMinimum, checkMaximum, includeMaximum)
        {
            _minValue = minValue.Date;
            _maxValue = maxValue.Date;
        }

        public override bool ValidateInput(object obj, BaseObject bo, ref string error)
        {
            bool result = false;
            try
            {
                DateTime value = System.Convert.ToDateTime(obj);
                result = CheckInput<DateTime>(value.Date, _minValue, _maxValue, ref error);
            }
            catch (Exception ex)
            {
                ShowConvertError(obj, "DateTime", ex);
                error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_Error", FieldName);
            }
            return result;
        }

        private readonly DateTime _minValue;
        private readonly DateTime _maxValue;
    }
}
