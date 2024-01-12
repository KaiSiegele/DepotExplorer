using Basics;
using Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VermoegensData
{
    public class JahrDataObject : BaseDataObject
    {
        public JahrDataObject(BaseObject jahr, Kurse kurse)
            : base(jahr)
        {
            _kurse = kurse;
        }

        protected override bool ExecuteCommand(CommandTyp commandTyp)
        {
            Debug.Assert(commandTyp == CommandTyp.Load || commandTyp == CommandTyp.Update);
            BaseObject.CheckParameter<Jahr>(commandTyp.ToString() + " Jahr", MainObject);
            return CommandInterpreter.ExecuteCommand(commandTyp, new JahrCommand(MainObject, _jahrQuery, CachedData.Jahre, _kursQuery, _kurse));
        }

        private readonly Kurse _kurse = null;
        private readonly KursQuery _kursQuery = new KursQuery();
        private readonly JahrQuery _jahrQuery = new JahrQuery();
    }
}
