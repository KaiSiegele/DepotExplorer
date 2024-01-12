using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basics;
using System.Diagnostics;
using Persistence;
using Tools;

namespace VermoegensData
{
    internal class KursQuery : BaseQuery
    {
        protected override bool Load(BaseObject bo, BaseObjects bos, Error error)
        {
            bool result = false;
            string field = string.Empty;
            string sortCriterium = string.Empty;
            if (bo is Wertpapier)
            {
                field = "Wertpapier";
                sortCriterium = "Jahr";
            }
            else if (bo is Jahr)
            {
                field = "Jahr";
                sortCriterium = "Wertpapier";
            }
            if (!string.IsNullOrEmpty(field))
            {
                string query = string.Format("select Id, Jahr, Wertpapier, Wert from Kurse where {0} = {1} order by {2}", field, bo.Id, sortCriterium);
                result = DatabaseHandle.LoadObjects<Kurs>(query, bos, error);
            }
            else
            {
                Log.Write(TraceLevel.Error, "KursQuery", "Load", bo.ToString() + " wird nicht unterstützt.");
            }
            return result;
        }

        protected override bool Load(BaseObject bo, Error error)
        {
            return true;
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            string query = @"insert into Kurse (Id, Jahr, Wertpapier, Wert) values (@id, @jahr, @wertpapier, @wert)";
            bool result = DatabaseHandle.Execute(query, bo, "Id,Jahr,Wertpapier,Wert", error);
            return result;
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Kurse set Jahr=@jahr, Wert=@wert where Id=@id and Wertpapier=@wertpapier";
            bool result = DatabaseHandle.Execute(query, bo, "Jahr,Wertpapier,Wert,Id", error);
            return result;
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Kurse where Id = @id and Wertpapier=@wertpapier";
            bool result = DatabaseHandle.Execute(query, bo, "Id,Wertpapier", error);
            return result;
        }
    }
}
