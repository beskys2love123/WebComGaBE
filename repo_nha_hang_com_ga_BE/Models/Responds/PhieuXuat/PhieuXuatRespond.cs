using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;

namespace repo_nha_hang_com_ga_BE.Models.Responds.PhieuXuat;

public class PhieuXuatRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenPhieu { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? nguoiNhan { get; set; }
    public string? lyDoXuat { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public IdName? nhanVien { get; set; }
    public List<loaiNguyenLieuXuatRespond>? loaiNguyenLieus { get; set; }

}

public class loaiNguyenLieuXuatRespond : IdName
{
    public List<nguyenLieuXuatRespond>? nguyenLieus { get; set; }
}
public class nguyenLieuXuatRespond
{
    public string? id { get; set; }
    public string? tenNguyenLieu { get; set; }
    public int? soLuong { get; set; }
    public IdName? donViTinh { get; set; }
    public int? soLuongBanDau { get; set; }

    public int? soLuongXuat { get; set; }
    public int? chenhLech { get; set; }

}


