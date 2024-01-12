using Basics;
using Persistence;
using System;
using System.Diagnostics;
using Tools;

namespace VermoegensData
{
    /// <summary>
    /// Metadaten enthalten Informationen ueber die
    /// Struktur der Datenbank und die verwendeten Objekt-Typen
    /// </summary>
    public static class MetaData
    {
        /// <summary>
        /// Prüft ob die Datenbank die richtige Version hat
        /// </summary>
        /// <param name="major">Erwartete Major-Version</param>
        /// <param name="minor">Erwartete Minor-Version</param>
        /// <param name="message">Fehlermeldung</param>
        /// <returns>true wenn die Version richtig ist, false sonst</returns>
        internal static bool CheckDatabaseVersion(int major, int minor, ref string message)
        {
            bool result = false;
            BaseObjects versions = new BaseObjects();

            ObjectsCommand loadVersions = new ObjectsCommand(Properties.Resources.DBVersion, new VersionQuery(), versions);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, loadVersions))
            {
                Version dbVersion = versions.GetObject<Version>(1);
                if (dbVersion != null)
                {
                    if (dbVersion.Major == major && dbVersion.Minor == minor)
                    {
                        result = true;
                    }
                    else
                    {
                        message = Properties.Resources.ResourceManager.GetMessageFromResource("DBVersionCheckingVersion_Error", major, minor, dbVersion.Major, dbVersion.Minor);
                    }
                }
                else
                {
                    message = Properties.Resources.DBVersionCheckingSyntax_Error;
                }
            }
            else
            {
                message = Properties.Resources.DBVersionLoading_Error;
            }

            if (result)
                Log.Write(TraceLevel.Info, "MetaData", "CheckDatabaseVersion", "Erfolgreich ausgefuehrt");
            else
                Log.Write(TraceLevel.Error, "MetaData", "CheckDatabaseVersion", "Fehlgeschlagen {0}", message);

            return result;
        }

        internal enum ObjektTypID
        {
            Depot = 1,
            Bank,
            Wertpapier,
            Bestand,
            Kurs,
            Zuordnung,
            Addresse
        };

        /// <summary>
        /// Gibt die nächste freie Id zurück für einen Typ
        /// zurück, wenn ein neues Objekt eingefuegt werden soll
        /// </summary>
        /// <param name="objeckTypID">Type des Objekts</param>
        /// <returns>Nächste Id, BaseObject.NotSpecified bei einem Fehler</returns>
        internal static int GetNextObjectId(ObjektTypID objeckTypID)
        {
            int nextId = BaseObject.NotSpecified;
            ObjektTyp objektTyp = new ObjektTyp();
            objektTyp.Id = (int)objeckTypID;

            ObjectCommand objectCommand = new ObjectCommand(objektTyp, query, objektTypen);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, objectCommand))
            {
                nextId = objektTyp.NextId;
                objektTyp.NextId++;
                objektTyp.ObjectState = ObjectState.Updated;
                if (!CommandInterpreter.ExecuteCommand(CommandTyp.Update, objectCommand))
                {
                    nextId = BaseObject.NotSpecified;
                }
            }
            Log.Write(TraceLevel.Info, "MetaData", "GetNextObjectId", "Next id for {0} is {1}", objeckTypID, nextId);
            return nextId;
        }

        /// <summary>
        /// Prüft ob, für jeden Objekt-Typ ein Eintrag in der
        /// Tabelle ObjektTypen vorhanden ist
        /// </summary>
        /// <param name="message">Fehlermeldung</param>
        /// <returns>Eintraege vorhanden</returns>
        internal static bool CheckObjektTypen(ref string message)
        {
            bool result = false;
            ObjectsCommand loadObjektTypen = new ObjectsCommand(Properties.Resources.ObjektTypen, query, objektTypen);
            if (CommandInterpreter.ExecuteCommand(CommandTyp.Load, loadObjektTypen))
            {
                result = true;
                var ids = Enum.GetValues(typeof(ObjektTypID));
                foreach (int id in ids)
                {
                    if (objektTypen[id] == null)
                        result = false;
                }
                if (!result)
                {
                    message = Properties.Resources.ObjektTypenChecking_Error;
                }
            }
            else
            {
                message = Properties.Resources.ObjektTypenLoading_Error;
            }

            if (result)
                Log.Write(TraceLevel.Info, "MetaData", "CheckObjektTypen", "Erfolgreich ausgefuehrt");
            else
                Log.Write(TraceLevel.Error, "MetaData", "CheckObjektTypen", "Fehlgeschlagen {0}", message);

            return result;
        }

        private static readonly BaseObjects objektTypen = new BaseObjects();
        private static readonly ObjektTpyQuery query = new ObjektTpyQuery();
        
        #region Database Version-Typen
        class Version : BaseObject
        {
            public int Major { get; set; }

            public int Minor { get; set; }

            public int Patch { get; set; }
        }

        class VersionQuery : BaseQuery
        {
            protected override bool Load(BaseObjects bos, Error error)
            {
                return DatabaseHandle.LoadObjects<Version>("SELECT * FROM Versionen", bos, error);
            }
        }
        #endregion

        #region Objekt-Typen
        class ObjektTyp : BaseObject
        {
            public int NextId { get; set; }
        }

        class ObjektTpyQuery : BaseQuery
        {
            protected override bool Load(BaseObjects bos, Error error)
            {
                return DatabaseHandle.LoadObjects<ObjektTyp>("SELECT * FROM ObjektTypen", bos, error);
            }

            protected override bool Load(BaseObject bo, Error error)
            {
                string query = @"SELECT Name, NextId FROM ObjektTypen where Id = @id";
                return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
            }

            protected override bool Update(BaseObject bo, Error error)
            {
                string query = @"update ObjektTypen set Name=@name, NextId=@nextId where Id = @id";
                return DatabaseHandle.Execute(query, bo, new string[] { "Name", "NextId", "Id" }, error);
            }
        }
        #endregion
    }
}