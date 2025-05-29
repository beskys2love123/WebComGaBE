using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace file.service.Models;

public class FileMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public ObjectId FileId { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}