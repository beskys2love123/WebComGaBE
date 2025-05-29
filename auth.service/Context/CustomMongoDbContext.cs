using MongoDB.Bson;
using MongoDbGenericRepository;

namespace auth.service.Context;

public class CustomMongoDbContext : MongoDbContext
{
    public CustomMongoDbContext(string connectionString, string dbName)
        : base(connectionString, dbName)
    {
    }

    protected override void InitializeGuidRepresentation()
    {
    }

    public override void SetGuidRepresentation(GuidRepresentation guidRepresentation)
    {
    }
}