using Basics;
using Persistence;


namespace VermoegensData
{
    internal class WertpapierQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            return Load(string.Empty, bos, error);
        }

        protected override bool Load(string condition, BaseObjects bos, Error error)
        {
            string query = @"SELECT wp.Id as Id, wp.Name as Name, wp.WKN as WKN, wp.ISIN as ISIN FROM Wertpapiere wp";
            if (!string.IsNullOrEmpty(condition))
            {
                query += string.Format(" where {0}", condition);
            }
            return DatabaseHandle.LoadObjects<Wertpapier>(query, bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            const string query = @"SELECT Name, Art, WKN, ISIN FROM Wertpapiere where Id = @id";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            const string query = @"insert into Wertpapiere (Id, Name, Art, WKN, ISIN) values (@id, @name, @art, @wKN, @iSIN)";
            return DatabaseHandle.Execute(query, bo, "Id,Name,Art,WKN,ISIN", error); ;
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            const string query = @"update Wertpapiere set Name=@name, WKN=@wKN, ISIN=@iSIN where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Name,WKN,ISIN,Id", error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Wertpapiere where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }
    }
}
