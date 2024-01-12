using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Basics;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Tools;

namespace Persistence
{
    /// <summary>
    /// Hilfsklasse um gemeinsame Funktionalität von
    /// SQLCeDatabase und SQLServerDatabase zu nutzen
    /// </summary>
    internal static class DatabaseTools
    {
        public static void HandleError(string className, string method, string message, Error error)
        {
            if (error != null)
                error.Description = message;
            Log.Write(TraceLevel.Error, className, method, message);
        }

        public static void HandleError(string className, string method, Exception ex, Error error)
        {
            if (error != null)
                error.Description = ex.Message;
            Log.Write(className, method, ex);
        }

        public static bool LoadObject(DbDataReader reader, BaseObject bo, Error error)
        {
            bool result = false, found = false;
            while (reader.Read() && !found)
            {
                if (UpdateObject(bo, reader))
                {
                    result = true;
                }
                else
                {
                    HandleError(ownClassName, "LoadObject", "Fehler beim Aktualiseren von " + bo.ToString(), error);
                }
                found = true;
            }
            if (!found)
                HandleError(ownClassName, "LoadObject", bo.ToString() + " nicht gefunden", error);

            return result;
        }

        public static bool LoadObjects<T>(DbDataReader reader, string query, BaseObjects bos, Error error)
        {
            bool result = true;
            while (reader.Read())
            {
                object obj = Activator.CreateInstance(typeof(T));
                BaseObject bo = obj as BaseObject;
                if (bo != null)
                {
                    if (UpdateObject(bo, reader))
                    {
                        if (!bos.AddObject(bo))
                        {
                            result = false;
                            HandleError(ownClassName, "LoadObjects", "Fehler beim Einfügen von " + bo.ToString(), error);
                        }
                    }
                    else
                    {
                        result = false;
                        HandleError(ownClassName, "LoadObjects", "Fehler beim Aktualiseren von " + bo.ToString(), error);
                    }
                }
            }
            return result;
        }

        static bool UpdateObject(BaseObject bo, IDataRecord record)
        {
            bool result = true;
            int fields = record.FieldCount;
            for (int i = 0; i < fields; i++)
            {
                object value = record.GetValue(i);
                if (value == DBNull.Value)
                {
                    value = null;
                }
                if (!bo.SetValue(record.GetName(i), record.GetFieldType(i), value))
                {
                    result = false;
                }
            }
            return result;
        }

        private static readonly string ownClassName = "DatabaseTools";
    }
}
