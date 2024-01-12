using Basics;
using Persistence;

namespace VermoegensData
{
    internal class AktieQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            string query = @"SELECT wp.Id as Id, wp.Name as Name, wp.WKN as WKN, wp.ISIN as ISIN, Land, Branche, Beschreibung FROM Wertpapiere wp inner join Aktien a on wp.Id = a.Id";
            return DatabaseHandle.LoadObjects<Aktie>(query, bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            string query = @"SELECT wp.Id as Id, wp.Name as Name, wp.WKN as WKN, wp.ISIN as ISIN, Land, Branche, Beschreibung FROM Wertpapiere wp inner join Aktien a on wp.Id = a.Id where a.Id = @id";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Aktien (Id, Land, Branche, Beschreibung) values (@id, @land, @branche, @beschreibung)";
            return DatabaseHandle.Execute(query, bo, "Id,Land,Branche,Beschreibung", error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Aktien set Land=@land, Branche=@branche, Beschreibung=@beschreibung where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Land,Branche,Beschreibung,Id", error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Aktien where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }
    }
}
