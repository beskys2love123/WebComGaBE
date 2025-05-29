using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Responds.NguyenLieu;

public class NguyenLieuRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public string? tenNguyenLieu { get; set; }

    public string? moTa { get; set; }

    public DateTimeOffset? hanSuDung { get; set; }
    public int? soLuong { get; set; }

    public IdName? loaiNguyenLieu { get; set; }

    public IdName? donViTinh { get; set; }

    public IdName? tuDo { get; set; }

    public TrangThaiNguyenLieu? trangThai { get; set; }
}