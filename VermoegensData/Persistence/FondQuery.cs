using Basics;
using Persistence;

namespace VermoegensData
{
    internal class FondQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            string query = @"SELECT wp.Id as Id, wp.Name as Name, wp.WKN as WKN, wp.ISIN as ISIN, Typ, Theme, Region, Anbieter FROM Wertpapiere wp inner join Fonds f on wp.Id = f.Id";
            return DatabaseHandle.LoadObjects<Fond>(query, bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            string query = @"SELECT wp.Name as Name, wp.WKN as WKN, wp.ISIN as ISIN, Typ, Theme, Region, Anbieter FROM Wertpapiere wp inner join Fonds f on wp.Id = f.Id where f.Id = @id";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Fonds (Id, Typ, Theme, Region, Anbieter) values (@id, @typ, @theme, @region, @anbieter)";
            return DatabaseHandle.Execute(query, bo, "Id,Typ,Theme,Region,Anbieter", error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Fonds set Typ=@typ, Theme=@theme, Region=@region, Anbieter=@anbieter where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Typ,Theme,Region,Anbieter,Id", error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Fonds where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }
    }
}
 
