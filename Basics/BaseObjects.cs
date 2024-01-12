using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Tools;

namespace Basics
{
    /// <summary>
    /// Signatur fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Objekt eingefügt wurde
    /// </summary>
    /// <param name="id">Id des eingefügten Objekts</param>
    /// <param name="baseObject">Eingefügtes Objekt</param>
    public delegate void OnBaseObjectInserted(int id, BaseObject baseObject);

    /// <summary>
    /// Signatur fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Objekt geändert wurde
    /// </summary>
    /// <param name="id">Id des geänderten Objekts</param>
    /// <param name="baseObject">Geändertes Objekt</param>
    public delegate void OnBaseObjectUpdated(int id, BaseObject baseObject);

    /// <summary>
    /// Signatur fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Objekt gelöscht oder als gelöscht gekennzeichnet wurde
    /// </summary>
    /// <param name="id">Id des gelöschten Objekts</param>
    public delegate void OnBaseObjectRemoved(int id);

    /// <summary>
    /// Signatur für eine Callback-Methode, um zu erfahren,
    /// dass Objekte (aus der Datenbank) geladen werden
    /// </summary>
    public delegate void OnBaseObjectsLoading(BaseObject bo);

    /// <summary>
    /// Signatur für eine Callback-Methode, um zu erfahren,
    /// dass Objekte (aus der Datenbank) geladen wurden
    /// </summary>
    public delegate void OnBaseObjectsLoaded(BaseObject bo);


    /// <summary>
    /// Signatur für eine Callback-Methode, um zu erfahren,
    /// dass alle Objekte gelöscht wurden
    /// </summary>
    public delegate void OnBaseObjectsCleared();

    /// <summary>
    /// Klasse, um mehrere BaseObject-Objekte zu verwalten
    /// Die Objekte werden in einem Dictionary verwaltet.
    /// Über die eindeutige Id kann auf die einzelnen Objekte
    /// zugegriffen werden.
    /// </summary>
    public class BaseObjects : Dictionary<int, BaseObject>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseObjects()
        {
        }

        #region Methoden, um ueber die Id den Namen zu erfragen und umgekehrt
        /// <summary>
        /// Gibt die Id des Objekts mit dem übergebenen Namen zurück
        /// </summary>
        /// <param name="name">Name des Objekts</param>
        /// <returns>Id des Objekts, NotSpecified falls kein Objekt vorhanden</returns>
        public int GetObjectId(string name)
        {
            int id;
            BaseObject bo = GetSingleObject<BaseObject>(b => b.Name == name);
            if (bo != null)
            {
                id = bo.Id;
            }
            else
            {
                id = BaseObject.NotSpecified;
            }
            return id;
        }

        /// <summary>
        /// Gibt den Namen des Objekts mit der übergebenen Id zurück
        /// </summary>
        /// <param name="id">Id des Objekts</param>
        /// <returns>Name des Objekts, string.Empty falls kein Objekt vorhanden</returns>
        public string GetObjectName(int id)
        {
            BaseObject bo;
            if (TryGetValue(id, out bo))
            {
                return bo.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gibt zu allen übergebenenen Ids die Namen der zugehörigen BaseObject-Objekte zurück
        /// </summary>
        /// <param name="ids">Eindeutige Ids</param>
        /// <returns>Liste mit den Objektnamen</returns>
        public List<string> GetObjectNames(IEnumerable<int> ids)
        {
            return (from id in ids select GetObjectName(id)).ToList();
        }

        /// <summary>
        /// Gibt eine Liste der Namen der Objekte zurück
        /// </summary>
        public List<string> ObjectNames
        {
            get
            {
                return (from kvp in this select kvp.Value.Name).ToList();
            }
        }

        /// <summary>
        /// Gibt das Object mit dem übergebenen Namen zurück
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Gefundenes Objekt</returns>
        public BaseObject this[string name]
        {
            get
            {
                return GetSingleObject<BaseObject>(b => b.Name == name);
            }
        }
        #endregion

        #region Template-Methoden fuer die Suche
        /// <summary>
        /// Gibt das Objekt mit der übergebenen Id zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des Objekts</typeparam>
        /// <param name="id">Eindeutige Id</param>
        /// <returns>Gefundenes Objekt, null falls nicht vorhanden oder Typ falsch ist</returns>
        public T GetObject<T>(int id) where T : BaseObject
        {
            BaseObject bo;
            if (TryGetValue(id, out bo))
            {
                return bo as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gibt zu allen übergebenen Ids die zugehörigen Objekte zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ der BaseObject-Objekte</typeparam>
        /// <param name="ids">Eindeutige Ids</param>
        /// <returns>Gefundene Objekte</returns>
        public IEnumerable<T> GetObjects<T>(IEnumerable<int> ids) where T : BaseObject
        {
            foreach (int id in ids)
            {
                T obj = GetObject<T>(id);
                if (obj != null)
                {
                    yield return obj;
                }
            }
        }

        /// <summary>
        /// Gibt alle Objekte der Liste mit der gesuchten Eigenschaft zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des Objekts</typeparam>
        /// <param name="praed">Gesuchte Eigenschaft</param>
        /// <returns>Gefundene Objekte</returns>
        public IEnumerable<T> GetObjects<T>(Func<T, bool> praed) where T : BaseObject
        {
            return (from bo in Values.OfType<T>() where praed(bo) select bo);
        }

        /// <summary>
        /// Stellt fest, ob sich in der Liste ein Objekt mit der gesuchten
        /// Eigenschaft befindet
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des Objekts</typeparam>
        /// <param name="praed">Gesuchte Eigenschaft</param>
        /// <returns>Es existiert ein Objekt mit der Eigenschaft</returns>
        public bool HasObject<T>(Func<T, bool> praed) where T : BaseObject
        {
            return CountObjects<T>(praed) > 0;
        }

        /// <summary>
        /// Gibt das erste Objekt aus der Liste mit der gesuchten Eigenschaft
        /// zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des Objekts</typeparam>
        /// <param name="praed">Gesuchte Eigenschaft</param>
        /// <returns>Gefundenes Objekt, null falls nicht vorhanden oder Typ falsch ist</returns>
        public T GetFirstObject<T>(Func<T, bool> praed) where T : BaseObject
        {
            return GetObjects<T>(praed).FirstOrDefault<T>();
        }

        /// <summary>
        /// Gibt das einzige Objekt aus der Liste mit der gesuchten Eigenschaft
        /// zurück
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des Objekts</typeparam>
        /// <param name="praed">Gesuchte Eigenschaft</param>
        /// <returns>Gefundenes Objekt, null falls nicht vorhanden oder Typ falsch ist</returns>
        public T GetSingleObject<T>(Func<T, bool> praed) where T : BaseObject
        {
            return GetObjects<T>(praed).SingleOrDefault<T>();
        }

        /// <summary>
        /// Zaehlt die Anzahl der Objekte mit der gesuchten Eigenschaft
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des BaseObject-Objekts</typeparam>
        /// <param name="praed">Gesuchte Eigenschaft</param>
        /// <returns>Anzahl der gefundenen Objekte</returns>
        public int CountObjects<T>(Func<T, bool> praed) where T : BaseObject
        {
            return GetObjects<T>(praed).Count();
        }

        /// <summary>
        /// Wendet die uebergebene Aktion auf alle Objekte an
        /// </summary>
        /// <typeparam name="T">Abgeleiteter Typ des BaseObject-Objekts</typeparam>
        /// <param name="action">Anzuwendende Aktion</param>
        public void UpdateObjects<T>(Action<T> action) where T : BaseObject
        {
            foreach (var kvp in this)
            {
                T objectToUpdate = kvp.Value as T;
                if (objectToUpdate != null)
                {
                    action(objectToUpdate);
                }
            }
        }
        #endregion


        #region Methoden um die Objekte zu veraendern
        /// <summary>
        /// Löscht alle Objekte und löst das Clear-Event aus
        /// </summary>
        public void ClearObjects()
        {
            if (Cleared != null)
            {
                Cleared();
            }
            Clear();
        }

        /// <summary>
        /// Fügt alle BaseObjects aus dem übergebenen BaseObjects-Objekt ein
        /// </summary>
        /// <param name="baseObjects">Objekt, dessen BaseObjects ingefügt werden sollen</param>
        /// <returns>Mindestens ein BaseObject konnte eingefügt werden</returns>
        public bool InsertObjects(BaseObjects baseObjects)
        {
            var bos = (from kvp in baseObjects select kvp.Value);
            return InsertObjects(bos);
        }

        /// <summary>
        ///  Fügt alle übergebenen Objekte ein und löst das LoadFinished-Event aus
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseObjects">Objekte, die eingefügt werden sollen</param>
        /// <returns>Mindestens ein Object konnte eingefügt werden</returns>
        public bool InsertObjects<T>(IEnumerable<T> baseObjects) where T : BaseObject
        {
            StartsLoadingObjects(null);
            bool result = false;
            foreach (BaseObject bo in baseObjects)
            {
                if (InsertObject(bo, false))
                {
                    result = true;
                }
            }
            LoadObjectsFinished(null);
            return result;
        }

        /// <summary>
        /// Versucht ein neues Objekt einzufügen und lösst
        /// bei Erfolg ein Inserted-Event aus. 
        /// Das Einfügen ist erfolgreich, wenn noch kein BaseObject-Objekt 
        /// mit der Id des uebergebenen Objekts vorhanden ist
        /// </summary>
        /// <param name="baseObject">Einzufügendes Objekt</param>
        /// <returns>Einfuegen erfolgreich</returns>
        public virtual bool InsertObject(BaseObject baseObject)
        {
            return InsertObject(baseObject, true);
        }

        /// <summary>
        /// Versucht ein neues Objekt einzufügen
        /// Das Einfügen ist erfolgreich, wenn noch kein Objekt 
        /// mit der Id des uebergebenen Objekts vorhanden ist
        /// </summary>
        /// <param name="baseObject">Einzufügendes Objekt</param>
        /// <returns>Einfuegen erfolgreich</returns>
        public virtual bool AddObject(BaseObject baseObject)
        {
            return InsertObject(baseObject, false);
        }

        /// <summary>
        /// Zeigt an, dass das Laden neuer Objekte
        /// beendet wurde.
        /// </summary>
        /// <param name="bo">Objekt zu dem die zu füllende Liste gehört</param>
        public void LoadObjectsFinished(BaseObject bo)
        {
            if (Loaded != null)
            {
                Loaded(bo);
            }
        }

        /// <summary>
        /// Zeigt an, dass das Laden neuer Objekte
        /// begonnen wird.
        /// </summary>
        /// <param name="bo">Objekt zu dem die zu füllende Liste gehört</param>
        public void StartsLoadingObjects(BaseObject bo)
        {
            if (Loading != null)
            {
                Loading(bo);
            }
        }

        /// <summary>
        /// Versucht ein Objekt zu aktualieren und lösst
        /// bei Erfolg ein Updated-Event aus. 
        /// Das Aktualisieren ist erfolgreich, wenn ein Objekt 
        /// mit der Id des uebergebenen Objekts vorhanden ist
        /// </summary>
        /// <param name="baseObject">Zu aktualisierendes Objekt</param>
        /// <returns>Aktualisierung erfolgreich</returns>
        public virtual bool UpdateObject(BaseObject baseObject)
        {
            int id = baseObject.Id;
            bool result = false;
            if (ContainsKey(id))
            {
                this[id] = baseObject;
                if (Updated != null)
                {
                    Updated(id, baseObject);
                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Versucht ein Objekt zu löschen und lösst
        /// bei Erfolg ein Removed-Event aus. 
        /// Das Löschen ist erfolgreich, wenn ein Objekt 
        /// mit der Id des uebergebenen Objekts vorhanden ist
        /// </summary>
        /// <param name="baseObject">Zu löschendes Objekt</param>
        /// <returns>Löschen erfolgreich</returns>
        public bool RemoveObject(BaseObject baseObject)
        {
            return RemoveObject(baseObject, true);
        }

        /// <summary>
        /// Markiert, das Objekt mit der übergebenen Id als gelöscht
        /// Das Löschen ist erfolgreich, wenn ein BaseObject-Objekt 
        /// mit der uebergebenen Id vorhanden ist
        /// </summary>
        /// <param name="id">Id des BaseObject-Objekts</param>
        /// <returns>Markierung erfolgreich</returns>
        public bool SetObjectAsRemoved(int id)
        {
            bool result;
            BaseObject bo;
            if (TryGetValue(id, out bo))
            {
                if (bo.ObjectState == ObjectState.Inserted)
                {
                    if (Remove(bo.Id))
                    {
                        result = true;
                        if (Removed != null)
                        {
                            Removed(id);
                        }
                    }
                    else
                    {
                        Log.Write(TraceLevel.Error, "BaseObjects", "SetObjectAsRemoved", "{0} kann nicht gelöscht werden", bo);
                        result = false;
                    }
                }
                else if (bo.ObjectState == ObjectState.Stored || bo.ObjectState == ObjectState.Updated)
                {
                    result = true;
                    bo.ObjectState = ObjectState.Removed;
                    if (Removed != null)
                    {
                        Removed(id);
                    }
                }
                else
                {
                    Log.Write(TraceLevel.Error, "BaseObjects", "SetObjectAsRemoved", "{0} mit ungültigen Status {1}", bo, bo.ObjectState);
                    result = false;
                }
            }
            else
            {
                Log.Write(TraceLevel.Error, "BaseObjects", "SetObjectAsRemoved", "{0} nicht gefunden", bo);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Zeigt an, dass alle vorhandenen Objekte erfolgreich in der Datenbank gespeichert wurden
        /// Alle als gelöscht gekennzeichneten Objekte werden gelöscht
        /// Bei den geänderten Objekte wird der Status zurückgesetzt
        /// </summary>
        public void SetObjectsAsStored()
        {
            List<BaseObject> bosDeleted = GetObjects<BaseObject>(bo => bo.Removed).ToList();
            foreach(var bo in bosDeleted)
            {
                RemoveObject(bo, false);
            }

            IEnumerable<BaseObject> bosChanged = GetObjects<BaseObject>(bo => bo.Changed);
            foreach (var bo in bosChanged)
            {
                bo.ObjectState = ObjectState.Stored;
            }
        }

        #endregion

        public OnBaseObjectInserted Inserted;

        public OnBaseObjectUpdated Updated;

        public OnBaseObjectRemoved Removed;

        public OnBaseObjectsLoaded Loaded;

        public OnBaseObjectsLoading Loading;

        public OnBaseObjectsCleared Cleared;

        /// <summary>
        /// Löscht das übergebene Objekt 
        /// </summary>
        /// <param name="baseObject">Zu löschendes Objekt</param>
        /// <param name="update">Zeigt an, ob bei Erfolg das Removed-Event ausgelöst werden soll</param>
        /// <returns>Objekte konnte gelöscht werden</returns>
        private bool RemoveObject(BaseObject baseObject, bool update)
        {
            int id = baseObject.Id;
            bool result = false;
            if (Remove(id))
            {
                result = true;
                if (update)
                {
                    if (Removed != null)
                    {
                        Removed(id);
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Fügt das übergebene Objekt ein
        /// </summary>
        /// <param name="baseObject">Einzufügendes Objekt</param>
        /// <param name="update">Zeigt an, ob bei Erfolg das Inserted-Event ausgelöst werden soll</param>
        /// <returns>Objekte konnte eingefügt werden</returns>
        private bool InsertObject(BaseObject baseObject, bool update)
        {
            int id = baseObject.Id;
            bool result = false;
            if (ContainsKey(id) == false)
            {
                Add(id, baseObject);
                if (update)
                {
                    if (Inserted != null)
                    {
                        Inserted(id, baseObject);
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
