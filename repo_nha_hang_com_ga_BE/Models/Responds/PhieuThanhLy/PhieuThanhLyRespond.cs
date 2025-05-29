using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;

namespace repo_nha_hang_com_ga_BE.Models.Responds.PhieuThanhLy;

public class PhieuThanhLyRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenPhieu { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public IdName? nhanVien { get; set; }
    public List<loaiNguyenLieuThanhLyRespond>? loaiNguyenLieus { get; set; }

}

public class loaiNguyenLieuThanhLyRespond : IdName
{
    public List<nguyenLieuThanhLyRespond>? nguyenLieus { get; set; }
}
public class nguyenLieuThanhLyRespond
{
    public string? id { get; set; }
    public string? tenNguyenLieu { get; set; }
    public int? soLuong { get; set; }
    public IdName? donViTinh { get; set; }
    public DateTimeOffset? hanSuDung { get; set; }
    
    public int? soLuongBanDau { get; set; }
    public int? soLuongThanhLy { get; set; }
    public int? chenhLech { get; set; }
    public string? lyDoThanhLy { get; set; }

}


