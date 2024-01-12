using UserInterface;
using System.Globalization;
using VermoegensData;

/// <summary>
/// Sämtliche Konverter ohne eigene Logik
/// </summary>
namespace DepotExplorer
{
    #region Konverter fuer die Stammdaten
    public class AnbieterConverter : BaseObjectConverter
    {
        public AnbieterConverter()
        {
            BaseObjects = MasterData.Anbieter;
            AddAll = true;
        }
    }

    public class LandConverter : BaseObjectConverter
    {
        public LandConverter()
        {
            BaseObjects = MasterData.Laender;
            AddAll = true;
        }
    }

    public class FondsThemeConverter : BaseObjectConverter
    {
        public FondsThemeConverter()
        {
            BaseObjects = MasterData.Themen;
            AddNotSpecified = true;
        }
    }

    public class FondsRegionConverter : BaseObjectConverter
    {
        public FondsRegionConverter()
        {
            BaseObjects = MasterData.Regionen;
            AddNotSpecified = true;
        }
    }
    #endregion

    #region Konverter fuer die zwischengespeicherten Daten
    public class DepotConverter : BaseObjectConverter
    {
        public DepotConverter()
        {
            BaseObjects = CachedData.Depots;
        }
    }

    public class JahrConverter : BaseObjectConverter
    {
        public JahrConverter()
        {
            BaseObjects =CachedData.Jahre;
        }
    }

    public class AktieConverter : BaseObjectConverter
    {
        public AktieConverter()
        {
            BaseObjects = CachedData.Aktien;
        }
    }

    public class FondConverter : BaseObjectConverter
    {
        public FondConverter()
        {
            BaseObjects = CachedData.Fonds;
        }
    }

    public class BankConverter : BaseObjectConverter
    {
        public BankConverter()
        {
            BaseObjects = CachedData.Banken;
        }
    }
    #endregion

    #region Konverter fuer die Enumerationen
    public class WertpapierArtConverter : BaseEnumConverter<WertpapierArt>
    {
    }
    public class FondsTypConverter : BaseEnumConverter<FondTyp>
    {
    }
    #endregion

    #region Konverter fuer die Dateneingabe
    public class AnteileConverter : BaseDoubleConverter
    {
        protected override string FormatDouble(double value)
        {
            NumberFormatInfo numberInfo = CultureInfo.CurrentCulture.NumberFormat;
            return value.ToString("F3", numberInfo);
        }
    }

    public class CurrencyConverter : BaseDoubleConverter
    {
        protected override string FormatDouble(double value)
        {
            NumberFormatInfo numberInfo = CultureInfo.CurrentCulture.NumberFormat;
            return value.ToString("F2", numberInfo);
        }
    }
    #endregion

}
