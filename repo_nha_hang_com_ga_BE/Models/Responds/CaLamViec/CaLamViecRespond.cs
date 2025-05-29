
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace repo_nha_hang_com_ga_BE.Models.Responds.CaLamViecRespond;
public class CaLamViecRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenCaLamViec { get; set; }
    public string? khungThoiGian { get; set; }
    public string? moTa { get; set; }

}