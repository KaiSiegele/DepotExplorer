using System.Diagnostics;
using Basics;
using Persistence.Properties;

namespace Persistence
{
    /// <summary>
    /// Befehl zum Lesen von Information aus der Datenbank und
    /// zum Schreiben in die Datenbank
    /// Die Informationen werden aus einem Objekt gelesen und in einem Objekt
    /// abgespeichert. Gleichzeitig wird die Liste von Objekten aktualisiert,
    /// in denen sich das Objekt befindet
    /// </summary>
    public class ObjectCommand : BaseCommand
    {
        /// <summary>
        /// Erezugt ein neues Objekt
        /// </summary>
        /// <param name="bo">Objekt um Daten aufzunehmen</param>
        /// <param name="bq">Abfrage, um in eine Tabelle einer Datenbank zu schreiben</param>
        /// <param name="bos">Liste von Objekten, in der sich das Objekt befindet</param>
        public ObjectCommand(BaseObject bo, BaseQuery bq, BaseObjects bos)
        {
            CheckParams(bo, bos, bq);
            _bo = bo;
            _bq = bq;
            _bos = bos;
        }

        #region Ueberschreibungen der Basisklasse
        public override string GetDescription(CommandTyp commandTyp)
        {
            string action;
            switch (commandTyp)
            {
                case CommandTyp.Load:
                    action = Resource.Load;
                    break;
                case CommandTyp.Insert:
                    action = Resource.Insert;
                    break;
                case CommandTyp.Update:
                    action = Resource.Update;
                    break;
                case CommandTyp.Remove:
                    action = Resource.Remove;
                    break;
                case CommandTyp.Refresh:
                    action = Resource.Refresh;
                    break;
                default:
                    action = string.Empty;
                    break;
            }
            return action + " " + _bo.ToString();
        }

        public override bool Execute(CommandTyp commandTyp, Error error)
        {
            bool result;
            switch (commandTyp)
            {
                case CommandTyp.Load:
                    result = ExecuteLoadObject(error);
                    break;
                case CommandTyp.Insert:
                    result = ExecuteInsertObject(error);
                    break;
                case CommandTyp.Update:
                    result = ExecuteUpdateObject(error);
                    break;
                case CommandTyp.Remove:
                    result = ExecuteRemoveObject(error);
                    break;
                case CommandTyp.Refresh:
                    result = ExecuteRefreshObject(error);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
        #endregion

        protected BaseObject BO
        {
            get
            {
                return _bo;
            }
        }

        protected BaseObjects BOS
        {
            get
            {
                return _bos;
            }
        }

        protected BaseQuery BQ
        {
            get
            {
                return _bq;
            }
        }

        /// <summary>
        /// Laedt Informationen aus einer Tabelle einer Datenbank,
        /// speichert sie im Objekt ab und aktualisiert die Liste
        /// der Objekte
        /// Vorbedingung: Das Objekt muss im neutralen Status sein
        /// </summary>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Informationen erfolgreich gelesen</returns>
        protected virtual bool ExecuteLoadObject(Error error)
        {
            CheckObjectState(ObjectState.Stored);
            bool result = _bq.LoadObject(_bo, error);
            return result;
        }

        /// <summary>
        /// Schreibt den Inhalt des Objekts in eine Tabelle
        /// einer Datenbank und aktualisiert die Liste der Objekte
        /// Vorbedingung: Das Objekt muss im Status Eingefuegt sein
        /// </summary>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Informationen erfolreich in die Tabelle eingefuegt</returns>
        protected virtual bool ExecuteInsertObject(Error error)
        {
            CheckObjectState(ObjectState.Inserted);
            bool result = _bq.InsertObject(_bo, error);
            if (result)
            {
                _bo.ObjectState = ObjectState.Stored;
                _bos.InsertObject(_bo);
            }
            return result;
        }

        /// <summary>
        /// Aktulisiert anhand des Inhalt des Objekts eine Tabelle
        /// einer Datenbank und aktualisiert die Liste der Objekte
        /// Vorbedingung: Das Objekt muss im Status Aktualisiert sein
        /// </summary>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Informationen erfolreich in der Tabelle aktualisiert</returns>
        protected virtual bool ExecuteUpdateObject(Error error)
        {
            bool result;
            if (ObjectState.Updated == _bo.ObjectState)
            {
                result = _bq.UpdateObject(_bo, error);
                if (result)
                {
                    _bo.ObjectState = ObjectState.Stored;
                    _bos.UpdateObject(_bo);
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Loeschten anhand des Inhalts des Objekts Daten aus einer Tabelle 
        /// einer Datenbank und aktualisiert die Liste der Objekte
        /// Vorbedingung: Das Objekt muss im Status Geloescht sein
        /// </summary>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Informationen erfolreich in der Tabelle aktualisiert</returns>
        protected virtual bool ExecuteRemoveObject(Error error)
        {
            CheckObjectState(ObjectState.Stored);
            bool result = _bq.RemoveObject(_bo, error);
            if (result)
            {
                _bos.RemoveObject(_bo);
            }
            return result;
        }

        /// <summary>
        /// Laedt Informationen aus einer Tabelle einer Datenbank,
        /// und aktualisert das Objekt und die Liste
        /// der Objekte
        /// Vorbedingung: Das Objekt muss im neutralen Status sein
        /// </summary>
        /// <param name="error">Fehler-Objekt</param>
        /// <returns>Informationen erfolgreich gelesen</returns>
        protected virtual bool ExecuteRefreshObject(Error error)
        {
            CheckObjectState(ObjectState.Stored);
            bool result = _bq.LoadObject(_bo, error);
            if (result)
            {
                _bos.UpdateObject(_bo);
            }
            else
            {
                _bos.RemoveObject(_bo);
            }
            return true;
        }

        /// <summary>
        /// Zeigt an, dass die Objekte die von dem Befehl bearbeiten
        /// wurden, erfolgreich in die Datenbank geschrieben wurden.
        /// </summary>
        protected virtual void SetObjectsStored()
        {
            _bo.ObjectState = ObjectState.Stored;
        }

        protected void FinishEditing(bool insert)
        {
            SetObjectsStored();
            BaseObject bo = _bo.Copy();
            if (insert)
            {
                _bos.InsertObject(bo);
            }
            else
            {
                _bos.UpdateObject(bo);
            }
        }

        [Conditional("DEBUG")]
        static void CheckParams(BaseObject bo, BaseObjects bos, BaseQuery bq)
        {
            Debug.Assert(!(bo == null), "Object nicht angegeben");
            Debug.Assert(!(bos == null), "Liste nicht angegeben");
            Debug.Assert(!(bq == null), "Query nicht angegeben");
        }

        [Conditional("DEBUG")]
        void CheckObjectState(ObjectState objectState)
        {
            Debug.Assert(_bo.ObjectState == objectState, string.Format("{0} hat State {1}, erwartet {2}", _bo, _bo.ObjectState, objectState));
        }

        private readonly BaseObject _bo;
        private readonly BaseObjects _bos;
        private readonly BaseQuery _bq;
    }
}