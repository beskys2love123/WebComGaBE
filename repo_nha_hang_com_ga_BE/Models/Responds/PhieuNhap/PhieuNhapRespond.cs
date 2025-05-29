using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;

namespace repo_nha_hang_com_ga_BE.Models.Responds.PhieuNhap;

public class PhieuNhapRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public string? tenPhieu { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? tenNguoiGiao { get; set; }
    public IdName? nhaCungCap { get; set; }
    public string? dienGiai { get; set; }
    public string? diaDiem { get; set; }
    public int? tongTien { get; set; }
    public string? ghiChu { get; set; }
    public IdName? nhanVien { get; set; }
    public List<nguyenLieuMenuRespond>? nguyenLieus { get; set; }
}

public class nguyenLieuMenuRespond
{
    public string? id { get; set; }
    public string? tenNguyenLieu { get; set; }
    public string? moTa { get; set; }

    public int? soLuong { get; set; }

    public DateTimeOffset? hanSuDung { get; set; }

    public IdName? loaiNguyenLieu { get; set; }

    public IdName? donViTinh { get; set; }

    public IdName? tuDo { get; set; }

    public TrangThaiNguyenLieu? trangThai { get; set; }

    public int? donGia { get; set; }
    public int? thanhTien { get; set; }
}


