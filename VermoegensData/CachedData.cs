using Persistence;
using Basics;
using Tools;
using System.Diagnostics;

namespace VermoegensData
{
    public static class CachedData
    {
        internal static bool Load(ref string message)
        {
            bool result = LoadJahre(ref message);
            result = result && LoadFonds(ref message);
            result = result && LoadAktien(ref message);
            result = result && LoadBanken(ref message);
            result = result && LoadDepots(ref message);

            if (result)
                Log.Write(TraceLevel.Info, "CachedData", "Load", "Erfolgreich ausgefuehrt");
            else
                Log.Write(TraceLevel.Error, "CachedData", "Load", "Fehlgeschlagen {0}", message);

            return result;
        }

        public static Banken Banken
        {
            get
            {
                return _banken;
            }
        }

        public static Aktien Aktien
        {
            get
            {
                return _aktien;
            }
        }

        public static Fonds Fonds
        {
            get
            {
                return _fonds;
            }
        }

        public static Jahre Jahre
        {
            get
            {
                return _jahre;
            }

        }

        public static Depots Depots
        {
            get
            {
                return _depots;
            }

        }

        #region Laden aus der Datenbank
        private static bool LoadBanken(ref string errorMessage)
        {
            return LoadCachedData(Properties.Resources.Banken, new BankQuery(), _banken, ref errorMessage);
        }

        private static bool LoadFonds(ref string errorMessage)
        {
            return LoadCachedData(Properties.Resources.Fonds, new FondQuery(), _fonds, ref errorMessage);
        }

        private static bool LoadAktien(ref string errorMessage)
        {
            return LoadCachedData(Properties.Resources.Aktien, new AktieQuery(), _aktien, ref errorMessage);
        }

        private static bool LoadJahre(ref string errorMessage)
        {
            return LoadCachedData(Properties.Resources.Jahre, new JahrQuery(), _jahre, ref errorMessage);
        }

        private static bool LoadDepots(ref string errorMessage)
        {
            return LoadCachedData(Properties.Resources.Depots, new DepotQuery(), _depots, ref errorMessage);
        }

        private static bool LoadCachedData(string objectsToLoad, BaseQuery bq, BaseObjects bos, ref string errorMessage)
        {
            bool result = false;
            ObjectsCommand loadData = new ObjectsCommand(objectsToLoad, bq, bos);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, loadData))
            {
                result = true;
            }
            else
            {
                errorMessage = Properties.Resources.ResourceManager.GetMessageFromResource("LoadingObjectsError", objectsToLoad);
            }
            return result;
        }
        #endregion

        private static readonly Banken _banken = new Banken();
        private static readonly Depots _depots = new Depots();
        private static readonly Aktien _aktien = new Aktien();
        private static readonly Fonds _fonds = new Fonds();
        private static readonly Jahre _jahre = new Jahre();
    }
}
