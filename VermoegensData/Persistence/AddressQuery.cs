using Basics;
using Persistence;

namespace VermoegensData
{
    internal class AddressQuery : BaseQuery
    {
        protected override bool Load(BaseObject bo, Error error)
        {
            string query = @"select Strasse, Hausnummer, PLZ, Ort, Land from Addressen where OwnerId=@ownerid and OwnerType=@ownertype";
            return DatabaseHandle.LoadObject(query, bo, new string[] { "OwnerId", "OwnerType" }, error);
        }

        protected override bool Insert(BaseObject bo, Error error)
        {
            //bo.Id = DatabaseHandle.NextId("Addressen");
            string query = @"insert into Addressen (Id, OwnerId, OwnerType, Strasse, Hausnummer, PLZ, Ort, Land) values (@id, @ownerid, @ownertype, @strasse, @hausnummer, @plz, @ort, @land)";
            return DatabaseHandle.Execute(query, bo, new string[] { "Id", "OwnerId", "OwnerType", "Strasse", "Hausnummer", "PLZ", "Ort", "Land" }, error);
        }

        protected override bool Update(BaseObject bo, Error error)
        {
            string query = @"update Addressen set Strasse=@strasse, Hausnummer=@hausnummer, PLZ=@plz, Ort=@ort, Land=@land where OwnerId=@ownerid and OwnerType=@ownertype";
            return DatabaseHandle.Execute(query, bo, new string[] { "Strasse", "Hausnummer", "PLZ", "Ort", "Land", "OwnerId", "OwnerType" }, error);
        }

        protected override bool Remove(BaseObject bo, Error error)
        {
            string query = @"delete from Addressen where OwnerId=@ownerid and OwnerType=@ownertype";
            return DatabaseHandle.Execute(query, bo, new string[] { "OwnerId", "OwnerType" }, error);
        }
    }
}
