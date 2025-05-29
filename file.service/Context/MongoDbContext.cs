using AspNetCore.Identity.MongoDbCore.Infrastructure;
using file.service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace file.service.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    public readonly IGridFSBucket _gridFS;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
        _gridFS = new GridFSBucket(_database);
    }

    public IMongoCollection<FileMetadata> Files => _database.GetCollection<FileMetadata>("Files");
}