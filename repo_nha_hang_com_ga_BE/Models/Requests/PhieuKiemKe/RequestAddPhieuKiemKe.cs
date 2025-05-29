using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.PhieuKiemKe;

public class RequestAddPhieuKiemKe
{
    public string? tenPhieu { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<loaiNguyenLieuKiemKe>? loaiNguyenLieus { get; set; }
}
