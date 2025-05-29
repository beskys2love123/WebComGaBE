using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.LoaiNguyenLieu;

public class LoaiNguyenLieuRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public string? tenLoai { get; set; }

    public string? moTa { get; set; }

    public IdName? danhMucNguyenLieu { get; set; }
}