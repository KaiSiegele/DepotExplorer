using Basics;
using Persistence;

namespace VermoegensData
{
    internal class StammdatumQuery : BaseQuery
    {
        public StammdatumQuery(string tableName)
               : base()

        {
            TableName = tableName;
        }

        protected override bool Load(BaseObjects bos, Error error)
        {
            return DatabaseHandle.LoadObjects<Stammdatum>(string.Format("SELECT * FROM {0}", TableName), bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            string query = string.Format("SELECT Name, Beschreibung FROM {0} where Id = @id", TableName);
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = string.Format("insert into {0} (Id, Name, Beschreibung) values (@id, @name, @beschreibung)", TableName);
            return DatabaseHandle.Execute(query, bo, new string[] { "Id", "Name", "Beschreibung" }, error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = string.Format("update {0} set Name=@name, Beschreibung=@beschreibung where Id = @id", TableName);
            return DatabaseHandle.Execute(query, bo, new string[] { "Id", "Name", "Beschreibung" }, error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = string.Format("delete from {0} where Id = @id", TableName);
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }

        private string TableName { get; set; }
    }
}
