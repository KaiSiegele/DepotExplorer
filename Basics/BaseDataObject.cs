
namespace Basics
{
    public enum CommandTyp
    {
        None,
        Load,
        Insert,
        Update,
        Remove,
        Refresh
    }

    /// <summary>
    /// Klasse für ein BaseObject und alle Informationen,
    /// die mit diesem Objekt zusammen abgespeichert werden
    /// </summary>
    public abstract class BaseDataObject
    {
        public BaseDataObject(BaseObject mainObject)
        {
            MainObject = mainObject;
        }

        public BaseObject MainObject { get; }

        public bool Load()
        {
            return ExecuteCommand(CommandTyp.Load);
        }

        public bool Insert()
        {
            return ExecuteCommand(CommandTyp.Insert);
        }

        public bool Update()
        {
            return ExecuteCommand(CommandTyp.Update);
        }

        public bool Remove()
        {
            return ExecuteCommand(CommandTyp.Remove);
        }

        public bool Refresh()
        {
            return ExecuteCommand(CommandTyp.Refresh);
        }

        protected abstract bool ExecuteCommand(CommandTyp commandTyp);        
    }
}

