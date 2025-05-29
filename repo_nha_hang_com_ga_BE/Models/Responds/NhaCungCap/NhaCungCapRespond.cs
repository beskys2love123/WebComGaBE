using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.NhaCungCap;

public class NhaCungCapRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenNhaCungCap { get; set; }
    public string? soDienThoai { get; set; }
    public string? diaChi { get; set; }



}