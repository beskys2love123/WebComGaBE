using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class PhieuXuat : BaseMongoDb
{

    public string? tenPhieu { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? nguoiNhan { get; set; }
    public string? lyDoXuat { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<loaiNguyenLieuXuat>? loaiNguyenLieus { get; set; }
}
public class loaiNguyenLieuXuat
{
    public string? id { get; set; }
    public List<nguyenLieuXuat>? nguyenLieus { get; set; }

}
public class nguyenLieuXuat
{
    public string? id { get; set; }
    public int? soLuongBanDau { get; set; }

    public int? soLuongXuat { get; set; }
    public int? chenhLech { get; set; }

}





