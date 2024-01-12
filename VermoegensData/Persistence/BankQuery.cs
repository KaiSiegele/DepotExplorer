using Persistence;
using Basics;

namespace VermoegensData
{
    internal class BankQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            return DatabaseHandle.LoadObjects<Bank>("SELECT * FROM Banken", bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            string query = @"SELECT Name, Beschreibung, Blz FROM Banken where Id = @id";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Banken (Id, Name, Beschreibung, Blz) values (@id, @name, @beschreibung, @blz)";
            return DatabaseHandle.Execute(query, bo, new string[] { "Id", "Name", "Beschreibung", "Blz" }, error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Banken set Name=@name, Beschreibung=@beschreibung, Blz=@blz where Id = @id";
            return DatabaseHandle.Execute(query, bo, new string[] { "Name", "Beschreibung", "Blz", "Id" }, error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Banken where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }
    }
}
