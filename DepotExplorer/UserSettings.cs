using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using UserInterface;
using Persistence;

namespace DepotExplorer
{
    /// <summary>
    /// Klasse fuer die Einstellungen fuer den Benutzer
    /// </summary>
    [Serializable]
    public class UserSettings
    {
        /// <summary>
        /// Liesst die Einstellungen aus einer Datei
        /// </summary>
        /// <param name="filepath">Pfad der Datei</param>
        /// <param name="us">Einstellungen</param>
        /// <returns>Einstellungen erfolgreich gelesen</returns>
        static public bool Load(string filepath, ref UserSettings us)
        {
            bool result = false;
            if (File.Exists(filepath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                FileStream sr = new FileStream(filepath, FileMode.Open);
                try
                {
                    us = (UserSettings)serializer.Deserialize(sr);
                    result = true;
                }
                catch (Exception ex)
                {
                    Prompts.ShowError(Properties.Resources.Title, ex.Message);
                }
            }
            else
            {
                Prompts.ShowError(Properties.Resources.Title, Properties.Resources.CannotFindUserSettings, filepath);
            }
            return result;
        }

        /// <summary>
        /// Sichert die Einstellungen in einer Datei
        /// </summary>
        /// <param name="filepath">Dateipfad</param>
        /// <param name="us">Zu sichernde Einstellungen</param>
        /// <returns>Einstellungen erfolgreich gesichert</returns>
        static public bool Save(string filepath, UserSettings us)
        {
            bool result = false;
            XmlSerializer ser = new XmlSerializer(typeof(UserSettings));
            System.IO.TextWriter wr = new StreamWriter(filepath);
            try
            {
                ser.Serialize(wr, us);
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                wr.Close();
            }
            return result;
        }

        public UserSettings()
        {
            DataBaseType = DBType.SQLCe;
            TraceLevel = TraceLevel.Error;
            LogDirectory = string.Empty;
            ConnectionString = @"Data Source=C:\Users\Siegele\Documents\Vermoegen.sdf;";
        }

        public DBType DataBaseType { get; set; }
        public TraceLevel TraceLevel { get; set; }
        public string LogDirectory { get; set; }
        public string ConnectionString { get; set; }
    }
}
