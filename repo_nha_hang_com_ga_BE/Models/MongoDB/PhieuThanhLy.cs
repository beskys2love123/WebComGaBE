using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class PhieuThanhLy : BaseMongoDb
{

    public string? tenPhieu { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<loaiNguyenLieuThanhLy>? loaiNguyenLieus { get; set; }

}
public class loaiNguyenLieuThanhLy
{
    public string? id { get; set; }
    public List<nguyenLieuThanhLy>? nguyenLieus { get; set; }

}
public class nguyenLieuThanhLy
{
    public string? id { get; set; }
    public int? soLuongBanDau { get; set; }
    public int? soLuongThanhLy { get; set; }
    public int? chenhLech { get; set; }
    public string? lyDoThanhLy { get; set; }

}





