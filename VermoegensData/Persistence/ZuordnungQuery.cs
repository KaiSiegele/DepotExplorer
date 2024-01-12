using Persistence;
using Basics;
using System.Diagnostics;
using Tools;

namespace VermoegensData
{
    internal class ZuordnungQuery : BaseQuery
    {

        protected override bool Load(BaseObject bo, BaseObjects bos, Error error)
        {
            bool result = false;
            if (bo is Depot)
            {
                string query = string.Format("select Id, Depot, Wertpapier from Zuordnungen where Depot = {0}", bo.Id);
                result = DatabaseHandle.LoadObjects<Zuordnung>(query, bos, error);
            }
            else if (bo is Wertpapier)
            {
                string query = string.Format("select Id, Depot, Wertpapier from Zuordnungen where Wertpapier = {0}", bo.Id);
                result = DatabaseHandle.LoadObjects<Zuordnung>(query, bos, error);
            }
            else
            { 
                Log.Write(TraceLevel.Error, "ZuordnungQuery", "Load", bo.ToString() + " wird nicht unterstützt.");
            }
            return result;
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Zuordnungen (Id, Depot, Wertpapier) values (@id, @depot, @wertpapier)";
            bool result = DatabaseHandle.Execute(query, bo, "Id,Depot,Wertpapier", error);
            return result;
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            return true;
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Zuordnungen where Id = @id";
            bool result = DatabaseHandle.Execute(query, bo, "Id", error);
            return result;
        }
    }
}
