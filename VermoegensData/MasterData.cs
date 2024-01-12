using System.Diagnostics;
using System.Linq;
using Basics;
using Persistence;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Stammdaten. Diese Daten werden nur gelesen
    /// und ändern sich sehr, sehr selten
    /// </summary>
    public static class MasterData
    {
        /// <summary>
        /// Liest die Stammdaten aus der Datenbank
        /// </summary>
        /// <param name="needData">Zeigt an, dass Daten vorhanden sein müssen</param>
        /// <param name="message">Fehlermeldung</param>
        /// <returns>Stammdaten wurden erfolgreich eingelesen</returns>
        internal static bool Load(bool needData, ref string message)
        {
            bool result = LoadAnbieter(needData, ref message);
            result = result && LoadLaender(needData, ref message);
            result = result && LoadRegionen(needData, ref message);
            result = result && LoadThemen(needData, ref message);

            if (result)
                Log.Write(TraceLevel.Info, "MasterData", "Load", "Erfolgreich ausgefuehrt");
            else
                Log.Write(TraceLevel.Error, "MasterData", "Load", "Fehlgeschlagen {0}", message);

            return result;
        }

        public static Stammdaten Anbieter
        {
            get
            {
                return anbieter;
            }
        }

        public static Stammdaten Laender
        {
            get
            {
                return laender;
            }
        }

        public static Stammdaten Regionen
        {
            get
            {
                return regionen;
            }
        }

        public static Stammdaten Themen
        {
            get
            {
                return themen;
            }
        }

        public static int DefaultAnbieterId
        {
            get
            {
                if (Anbieter != null)
                {
                    return Anbieter.Keys.FirstOrDefault();
                }
                else
                {
                    return BaseObject.NotSpecified;
                }
            }
        }

        public static int DefaultLandId
        {
            get
            {
                if (Laender != null)
                {
                    return Laender.Keys.FirstOrDefault();
                }
                else
                {
                    return BaseObject.NotSpecified;
                }
            }
        }

        #region Laden aus der Datenbank
        private static bool LoadAnbieter(bool needData, ref string message)
        {
            var messages = new LoadingParams(Properties.Resources.Anbieter, StammdatenTabelle.Anbieter, needData);
            return LoadStammDaten(messages, anbieter, ref message);
        }

        private static bool LoadLaender(bool needData, ref string message)
        {
            var messages = new LoadingParams(Properties.Resources.Laender, StammdatenTabelle.Laender, needData);
            return LoadStammDaten(messages, laender, ref message);
        }

        private static bool LoadRegionen(bool needData, ref string message)
        {
            var messages = new LoadingParams(Properties.Resources.Regionen, StammdatenTabelle.Regionen, needData);
            return LoadStammDaten(messages, regionen, ref message);
        }

        private static bool LoadThemen(bool needData, ref string message)
        {
            var messages = new LoadingParams(Properties.Resources.Themen, StammdatenTabelle.Themen, needData);
            return LoadStammDaten(messages, themen, ref message);
        }

        private class LoadingParams
        {
            public LoadingParams(string objectsToLoad, StammdatenTabelle tabelle, bool needData)
            {
                ObjectsToLoad = objectsToLoad;
                Tabelle = tabelle;
                NeedData = needData;
            }

            public string ObjectsToLoad { get; private set; }
            public StammdatenTabelle Tabelle { get; private set; }
            public bool NeedData { get; private set; }
        }

        private static bool LoadStammDaten(LoadingParams loadingParams, Stammdaten bos, ref string errorMessage)
        {
            bool result = false;
            BaseQuery bq = new StammdatumQuery(loadingParams.Tabelle.ToString());
            ObjectsCommand loadData = new ObjectsCommand(loadingParams.ObjectsToLoad, bq, bos);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, loadData))
            {
                if (bos.Any() || !loadingParams.NeedData)
                {
                    result = true;
                    bos.UpdateTabelle(loadingParams.Tabelle);
                }
                else
                {
                    errorMessage = Properties.Resources.ResourceManager.GetMessageFromResource("LoadingObjectsNoData", loadingParams.ObjectsToLoad);
                }
            }
            else
            {
                errorMessage = Properties.Resources.ResourceManager.GetMessageFromResource("LoadingObjectsError", loadingParams.ObjectsToLoad);
            }
            return result;
        }
        #endregion

        private static readonly Stammdaten anbieter = new Stammdaten();
        private static readonly Stammdaten laender = new Stammdaten();
        private static readonly Stammdaten regionen = new Stammdaten();
        private static readonly Stammdaten themen = new Stammdaten();
    }
}