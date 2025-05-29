using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace repo_nha_hang_com_ga_BE.Models.Common;

public class BaseMongoDb
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTimeOffset? createdDate { get; set; } = DateTimeOffset.UtcNow;

    public string? createdUser { get; set; } = "";

    public DateTimeOffset? updatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string? updatedUser { get; set; } = "";

    public bool? isDelete { get; set; } = false;
}