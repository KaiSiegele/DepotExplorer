using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Basics;

namespace UserInterface
{
  using System.Diagnostics;

  public class DoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (ConverterChecks.CheckTypes<double, string>(value, targetType))
      {
        NumberFormatInfo numberInfo = CultureInfo.CurrentCulture.NumberFormat;
//        double formatValue = Math.Round((double)value), 2);
        return ((double)value).ToString("F2", numberInfo);
        //return ((double) value).ToString("G", CultureInfo.CurrentCulture);
      }
      else
      {
        return Binding.DoNothing;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (ConverterChecks.CheckTypes<string, double>(value, targetType))
      {
        try
        {
          double conversion = System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
          return conversion;
        }
        catch (Exception ex)
        {
          Log.Write(TraceLevel.Warning, "ConvertBack", ex.Message);
          return Binding.DoNothing;
        }
      }
      else
      {
        return Binding.DoNothing;
      }
    }

    protected virtual string FormatDouble(double value)
    {
      return value.ToString("G", CultureInfo.CurrentCulture);
    }
  }
}
