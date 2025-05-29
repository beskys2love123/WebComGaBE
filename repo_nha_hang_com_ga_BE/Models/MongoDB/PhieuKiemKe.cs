using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class PhieuKiemKe : BaseMongoDb
{

    public string? tenPhieu { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public DateTimeOffset? ngayKiemKe { get; set; }
    public List<loaiNguyenLieuKiemKe>? loaiNguyenLieus { get; set; }
}
public class loaiNguyenLieuKiemKe
{
    public string? id { get; set; }
    public List<nguyenLieuKiemKe>? nguyenLieus { get; set; }

}
public class nguyenLieuKiemKe
{
    public string? id { get; set; }
    public int? soLuongThucTe { get; set; }
    public int? chenhLech { get; set; }
    public string? ghiChu { get; set; }
}





