using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basics;
using Persistence;

namespace VermoegensData
{
    public class DepotQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            return DatabaseHandle.LoadObjects<Depot>("SELECT * FROM Depots", bos, error);
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            string query = @"SELECT Name, Bank, KontoNummer FROM Depots where Id = @id";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "Id" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Depots (Id, Name, Bank, KontoNummer) values (@id, @name, @bank, @kontoNummer)";
            return DatabaseHandle.Execute(query, bo, new string[] { "Id", "Name", "Bank", "KontoNummer" }, error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Depots set Name=@name, Bank=@bank, KontoNummer=@kontoNummer where Id = @id";
            return DatabaseHandle.Execute(query, bo, new string[] { "Name", "Bank", "KontoNummer", "Id" }, error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Depots where Id = @id";
            return DatabaseHandle.Execute(query, bo, "Id", error);
        }
    }
}
