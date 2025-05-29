using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.KhuyenMai;

public class KhuyenMaiRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public string? tenKhuyenMai { get; set; }

    public DateTimeOffset ngayBatDau { get; set; }

    public DateTimeOffset ngayKetThuc { get; set; }

    public double giaTri { get; set; }
}