using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Persistence;
using Basics;
using System.Diagnostics;
using Tools;

namespace VermoegensData
{
    internal class BestandQuery : BaseQuery
    {
        protected override bool Load(BaseObject bo, BaseObjects bos, Error error)
        {
            bool result = false;
            if (bo is Depot)
            {
                string query = string.Format("select Id, Depot, Wertpapier, Jahr, Kauf, Verkauf, Anteile, Dividende from Bestaende where Depot = {0}", bo.Id);
                result = DatabaseHandle.LoadObjects<Bestand>(query, bos, error);
            }
            else
            {
                Log.Write(TraceLevel.Error, "BestandQuery", "Load", bo.ToString() + " wird nicht unterstützt.");
            }
            return result;
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Bestaende (Id, Depot, Wertpapier, Jahr, Anteile, Kauf, Verkauf, Dividende) values (@id, @depot, @wertpapier, @jahr, @anteile, @kauf, @verkauf, @dividende)";
            bool result = DatabaseHandle.Execute(query, bo, "Id,Depot,Wertpapier,Jahr,Anteile,Kauf,Verkauf,Dividende", error);
            return result;
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Bestaende set Jahr=@jahr, Wertpapier=@wertpapier, Anteile=@anteile, Kauf=@kauf, Verkauf=@verkauf, Dividende=@dividende where Id = @id and Depot=@depot";
            bool result = DatabaseHandle.Execute(query, bo, "Depot,Wertpapier,Jahr,Anteile,Kauf,Verkauf,Dividende,Id", error);
            return result;
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Bestaende where Id = @id and Depot=@depot";
            bool result = DatabaseHandle.Execute(query, bo, "Id,Depot", error);
            return result;
        }
    }
}
