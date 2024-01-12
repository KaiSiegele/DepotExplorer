
using System.Diagnostics;
using Basics;
using Persistence.Properties;

namespace Persistence
{
    /// <summary>
    /// Befehl zum Laden von Informationen aus der Datenbank,
    /// die in mehreren Objekten abgespeichert werden
    /// </summary>
    public class ObjectsCommand : BaseCommand
    {
        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="objectsDescription">Beschreibung der Objekte</param>
        /// <param name="bq">Abfrage um Informationen aus einer Datenbank zu lesen</param>
        /// <param name="bos">Liste von Objekten</param>
        public ObjectsCommand(string objectsDescription, BaseQuery bq, BaseObjects bos)
            : this(objectsDescription, bq, string.Empty, null, bos)
        {
        }

        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="objectsDescription">Beschreibung der untergeordneten Objekte</param>
        /// <param name="bq">Abfrage um Informationen aus einer Datenbank zu lesen</param>
        /// <param name="bo">Objekten zu dem Informationen gelesen werden sollen</param>
        /// <param name="bos">Liste von Objekten</param>
        public ObjectsCommand(string objectsDescription, BaseQuery bq, BaseObject bo, BaseObjects bos)
            : this(objectsDescription, bq, string.Empty, bo, bos)
        {
        }

        /// <summary>
        /// Erzeugt ein neues Objekt
        /// </summary>
        /// <param name="objectsDescription">Beschreibung der untergeordneten Objekte</param>
        /// <param name="bq">Abfrage um Informationen aus einer Datenbank zu lesen</param>
        /// <param name="condition">Abfragebedingung</param>
        /// <param name="bos">Liste von Objekten</param>
        public ObjectsCommand(string objectsDescription, BaseQuery bq, string condition, BaseObjects bos)
            : this(objectsDescription, bq, condition, null, bos)
        {
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
                default:
                    action = string.Empty;
                    break;
            }
            return action + " " + _objectsDescription;
        }

        public override bool Execute(CommandTyp commandTyp, Error error)
        {
            bool result;
            switch (commandTyp)
            {
                case CommandTyp.Load:
                    result = ExecuteLoadObjects(error);
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
        #endregion

        private ObjectsCommand(string objectsDescription, BaseQuery bq, string condition, BaseObject bo, BaseObjects bos)
        {
            CheckParams(objectsDescription, bos, bq);

            _objectsDescription = objectsDescription;
            _bq = bq;
            _condition = condition;
            _bo = bo;
            _bos = bos;
        }

        private bool ExecuteLoadObjects(Error error)
        {
            bool result;
            if(!string.IsNullOrEmpty(_condition))
            {
                result = _bq.LoadObjects(_condition, _bos, error);
            }
            else if (_bo != null)
            {
                result = _bq.LoadObjects(_bo, _bos, error);
            }
            else
            {
                result = _bq.LoadObjects(_bos, error);
            }

            return result;
        }

        [Conditional("DEBUG")]
        static void CheckParams(string objectsDescription, BaseObjects bos, BaseQuery bq)
        {
            Debug.Assert(!(string.IsNullOrEmpty(objectsDescription)), "Beschreibung nicht angegeben");
            Debug.Assert(!(bos == null), "Liste nicht angegeben");
            Debug.Assert(!(bq == null), "Query nicht angegeben");
        }

        private readonly BaseQuery _bq;
        private readonly BaseObject _bo;
        private readonly string _condition;
        private readonly BaseObjects _bos;
        private readonly string _objectsDescription;
    }
}