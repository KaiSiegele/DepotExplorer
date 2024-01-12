using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Basics
{
    /// <summary>
    /// Basisklasse für die Überprüfung, ob ein eingegebener
    /// Wert in einem angegegebenen Invervall liegt
    /// </summary>
    public abstract class BaseRangeValidation : BaseValidation
    {
        protected BaseRangeValidation(string fieldname, bool checkMinimum, bool includeMinimum, bool checkMaximum, bool includeMaximum)
          : base(fieldname)
        {
            CheckMinimum = checkMinimum;
            IncludeMinimum = includeMinimum;
            CheckMaximum = checkMaximum;
            IncludeMaximum = includeMaximum;
        }

        protected bool CheckMinimum { get; private set; }
        protected bool IncludeMinimum { get; private set; }
        protected bool CheckMaximum { get; private set; }
        protected bool IncludeMaximum { get; private set; }

        /// <summary>
        /// Prüft, ob der eingegebene Wert innerhalb des Intervalls liegt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">Eingabe</param>
        /// <param name="minValue">Minimum</param>
        /// <param name="maxValue">Maximum</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>true, wenn Eingabe im Intervall liegt, false sonst</returns>
        protected bool CheckInput<T>(T input, T minValue, T maxValue, ref string error) where T : IComparable<T>
        {
            bool result = true;
            if (this.CheckMinimum)
            {
                int compareResult = input.CompareTo(minValue);
                if (this.IncludeMinimum)
                {
                    if (compareResult < 0)
                    {
                        error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_MayNotSmallerThanValue", FieldName, minValue);
                        result = false;
                    }
                }
                else
                {
                    if (compareResult <= 0)
                    {
                        error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_MustBiggerThanValue", FieldName, minValue);
                        result = false;
                    }
                }
            }
            if (result && this.CheckMaximum)
            {
                int compareResult = input.CompareTo(maxValue);
                if (this.IncludeMaximum)
                {
                    if (compareResult > 0)
                    {
                        error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_MayNotBiggerThanValue", FieldName, maxValue);
                        result = false;
                    }
                }
                else
                {
                    if (compareResult >= 0)
                    {
                        error = Properties.Resource.ResourceManager.GetMessageFromResource("NumericValidation_MustSmallerThanValue", FieldName, maxValue);
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}
