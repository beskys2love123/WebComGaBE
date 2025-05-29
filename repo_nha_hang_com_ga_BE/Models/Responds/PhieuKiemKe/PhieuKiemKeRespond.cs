using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;

namespace repo_nha_hang_com_ga_BE.Models.Responds.PhieuKiemKe;

public class PhieuKiemKeRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenPhieu { get; set; }
    public string? diaDiem { get; set; }
    public DateTimeOffset? ngayKiemKe { get; set; }
    public string? ghiChu { get; set; }
    public IdName? nhanVien { get; set; }
    public List<loaiNguyenLieuKiemKeRespond>? loaiNguyenLieus { get; set; }

}

public class loaiNguyenLieuKiemKeRespond : IdName
{
    public List<nguyenLieuKiemKeRespond>? nguyenLieus { get; set; }
}
public class nguyenLieuKiemKeRespond
{
    public string? id { get; set; }
    public string? tenNguyenLieu { get; set; }
    public int? soLuong { get; set; }
    public int? soLuongThucTe { get; set; }
    public int? chenhLech { get; set; }
    public string? ghiChu { get; set; }

}


