using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.BangGia;

public class BangGiaRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public IdName? monAn { get; set; }

    public string? tenGia { get; set; }
    public int? giaTri { get; set; }
    public DateTime? createdDate { get; set; }

}