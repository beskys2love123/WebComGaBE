using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.PhieuXuat;

public class RequestUpdatePhieuXuat
{
    public string? tenPhieu { get; set; }
    public string? nguoiNhan { get; set; }
    public string? lyDoXuat { get; set; }
    public string? diaDiem { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<loaiNguyenLieuXuat>? loaiNguyenLieus { get; set; }
}