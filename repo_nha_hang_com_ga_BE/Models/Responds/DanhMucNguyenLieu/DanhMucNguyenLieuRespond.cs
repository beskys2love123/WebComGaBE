using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;

public class DanhMucNguyenLieuRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    
    public string? tenDanhMuc { get; set; }
    
    public string? moTa { get; set; }
}