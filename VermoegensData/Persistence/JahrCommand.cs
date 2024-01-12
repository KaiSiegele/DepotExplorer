using Basics;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VermoegensData
{
    internal class JahrCommand : ObjectCommand
    {
        public JahrCommand(BaseObject bo, BaseQuery bq, BaseObjects bos, KursQuery kursQuery, Kurse kurse)
            : base(bo, bq, bos)
        {
            _kursQuery = kursQuery;
            _kurse = kurse;
        }

        protected override bool ExecuteLoadObject(Error error)
        {
            return _kursQuery.LoadObjects(BO, _kurse, error);
        }

        protected override bool ExecuteUpdateObject(Error error)
        {
            bool result = false;
            string description = GetDescription(CommandTyp.Update);
            if (DatabaseHandle.StartTransaction(description, error))
            {
                if (_kursQuery.ProcessAllObjects(_kurse, error))
                {
                    result = true;
                    _kurse.SetObjectsAsStored();
                }
                DatabaseHandle.FinishTransaction(description, result, error);
            }
            return result;
        }

        private readonly KursQuery _kursQuery;
        private readonly Kurse _kurse;
    }
}
