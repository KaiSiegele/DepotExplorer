using Basics;
using Persistence;

namespace VermoegensData
{
    internal class StammdatumQuery : BaseQuery
    {
        public StammdatumQuery(string tableName)
               : base()

        {
            TableName = tableName;
        }

        protected override bool Load(BaseObjects bos, Error error)
        {
            return DatabaseHandle.LoadObjects<Stammdatum>(string.Format("SELECT * FROM {0}", TableName), bos, error);
        }

        private string TableName { get; set; }
    }
}
