using System.Collections.Generic;
using Basics;
using System.Diagnostics;
using Tools;

namespace Persistence
{
    public class Error
    {
        public string Description { get; set; }
    }

    /// <summary>
    /// Abstrakte Basisklasse fuer eine Abfrage
    /// Eine Abfrage schreibt in oder liest aus einer Tabelle
    /// einer Datenbank
    /// </summary>
    public abstract class BaseQuery
    {
        /// <summary>
        /// Liest Informationen aus einer Tabelle und füllt den Inhalt in die übergebenene Liste von Objekten
        /// </summary>
        /// <param name="bos">Zu füllende Liste mit Objekten</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Kein Fehler beim Füllen der Liste mit Objekten</returns>
        public bool LoadObjects(BaseObjects bos, Error error)
        {
            bool result = false;
            bos.Clear();
            bos.StartsLoadingObjects(null);
            if (Load(bos, error))
            {
                result = true;
            }
            bos.LoadObjectsFinished(null);
            return result;
        }

        /// <summary>
        /// Liest Informationen aus einer Tabelle und füllt den Inhalt in die übergenene Liste von Objekten
        /// </summary>
        /// <param name="bo">Objekt zu dem die zu füllende Liste gehört</param>
        /// <param name="bos">Zu füllende Liste mit Objekten</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Kein Fehler beim Füllen der Liste mit Objekten</returns>
        public bool LoadObjects(BaseObject bo, BaseObjects bos, Error error)
        {
            bool result = false;
            bos.Clear();
            bos.StartsLoadingObjects(bo);
            if (Load(bo, bos, error))
            {
                result = true;
            }
            bos.LoadObjectsFinished(bo);
            return result;
        }

        /// <summary>
        /// Liest Informationen aus einer Tabelle und füllt den Inhalt in die übergenene Liste von Objekten
        /// </summary>
        /// <param name="condition">Abfragebedingung</param>
        /// <param name="bos">Zu füllende Liste mit Objekten</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Kein Fehler beim Füllen der Liste mit Objekten</returns>
        public bool LoadObjects(string condition, BaseObjects bos, Error error)
        {
            bool result = false;
            bos.Clear();
            bos.StartsLoadingObjects(null);
            if (Load(condition, bos, error))
            {
                result = true;
            }
            bos.LoadObjectsFinished(null);
            return result;
        }

        /// <summary>
        /// Liest einen Datensatz aus der Tabelle und füllt den Inhalt in das übergebene Objekt
        /// </summary>
        /// <param name="bo">Zu füllendes Objekt</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Objekt erfolgreich gefüllt</returns>
        public bool LoadObject(BaseObject bo, Error error)
        {
            bool result = Load(bo, error);
            if (!result)
            {
                TraceError("LoadObject", bo, error);
            }
            return result;
        }
  
        /// <summary>
        /// Erzeugt einen neuen Datensatz der Tabelle anhand 
        /// der Informationen aus dem übergebenen Objekt
        /// </summary>
        /// <param name="bo">Objekt mit den notwendigen Informationen</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <param name="trace"></param>
        /// <returns>Neuer Datensatz erfolgreich eingefuegt</returns>
        public bool InsertObject(BaseObject bo, Error error)
        {
            bool result = Insert(bo, error);
            if (!result)
            {
                TraceError("InsertObject", bo, error);
            }
            return result;
        }

        /// <summary>
        /// Aktualisert einen Datensatz der Tabelle anhand 
        /// der Informationen aus dem übergebenen Objekt
        /// </summary>
        /// <param name="bo">Objekt mit den notwendigen Informationen</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <param name="trace"></param>
        /// <returns>Datensatz erfolgreich aktualisiert</returns>
        public bool UpdateObject(BaseObject bo, Error error)
        {
            bool result = Update(bo, error);
            if (!result)
            {
                TraceError("UpdateObject", bo, error);
            }
            return result;
        }

        /// <summary>
        /// Loescht den Datensatz der Tabelle der 
        /// zum übergebenen Objekt gehört
        /// </summary>
        /// <param name="bo">Objekt dessen zugehoeriger Datensatz gelöscht werden soll</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Datensatz erfolgreich aktualisiert</returns>
        public bool RemoveObject(BaseObject bo, Error error)
        {
            bool result = Remove(bo, error);
            if (!result)
            {
                TraceError("RemoveObject", bo, error);
            }
            return result;
        }

        /// <summary>
        /// Verändert die Datensätze der Tabelle anhand 
        /// der Informationen aus der übergebenenen Liste 
        /// </summary>
        /// <param name="bos">Liste mit Informationen</param>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Alle Datensätze wurden erfolgreich veraendert</returns>
        public bool ProcessAllObjects(BaseObjects bos, Error error)
        {
            bool result = true;
            foreach (KeyValuePair<int, BaseObject> kvp in bos)
            {
                bool processed;
                BaseObject bo = kvp.Value;

                switch (bo.ObjectState)
                {
                    case ObjectState.Inserted:
                        processed = InsertObject(bo, error);
                        break;
                    case ObjectState.Updated:
                        processed = UpdateObject(bo, error);
                        break;
                    case ObjectState.Removed:
                        processed = RemoveObject(bo, error);
                        break;
                    default:
                        processed = true;
                        break;
                }
                if (!processed)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #region Methoden, die in den abgeleiteten Klassen ueberschrieben werden koennen
        protected virtual bool Load(BaseObjects bos, Error error)
        {
            return false;
        }

        protected virtual bool Load(BaseObject bo, BaseObjects bos, Error error)
        {
            return false;
        }

        protected virtual bool Load(string condition, BaseObjects bos, Error error)
        {
            return false;
        }

        protected virtual bool Load(BaseObject bo, Error error)
        {
            return false;
        }

        protected virtual bool Insert(BaseObject bo, Error error)
        {
            return true;
        }

        protected virtual bool Update(BaseObject bo, Error error)
        {
            return true;
        }

        protected virtual bool Remove(BaseObject bo, Error error)
        {
            return true;
        }
        #endregion

        private void TraceError(string method, BaseObject bo, Error error)
        {
            Log.Write(TraceLevel.Warning, GetType().Name, method, "Fehler fuer {0}: {1}", bo, error.Description);
        }
    }
}
