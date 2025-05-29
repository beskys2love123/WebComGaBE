using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.PhieuNhap;

public class RequestUpdatePhieuNhap
{
    public string? tenPhieu { get; set; }
    public string? tenNguoiGiao { get; set; }
    public string? nhaCungCap { get; set; }
    public string? dienGiai { get; set; }
    public string? diaDiem { get; set; }
    public int? tongTien { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<nguyenLieuMenu>? nguyenLieus { get; set; }
}