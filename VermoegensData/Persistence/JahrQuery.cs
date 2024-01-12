using System;
using Basics;
using Persistence;


namespace VermoegensData
{
    internal class JahrQuery : BaseQuery
    {
        protected override bool Load(BaseObjects bos, Error error)
        {
            string query = string.Format("SELECT * FROM Jahre where Id < {0}", DateTime.Today.Year - 2000);

            return DatabaseHandle.LoadObjects<Jahr>(query, bos, error);
        }

    }
}
