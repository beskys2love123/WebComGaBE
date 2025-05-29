using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;
public class PhieuNhap : BaseMongoDb
{


    public string? tenPhieu { get; set; }
    public string? tenNguoiGiao { get; set; }
    public DateTimeOffset? ngayLap { get; set; }
    public string? nhaCungCap { get; set; }
    public string? dienGiai { get; set; }
    public string? diaDiem { get; set; }
    public int? tongTien { get; set; }
    public string? ghiChu { get; set; }
    public string? nhanVien { get; set; }
    public List<nguyenLieuMenu>? nguyenLieus { get; set; }
}


public class nguyenLieuMenu : RequestAddNguyenLieu
{
    public string? id { get; set; }
    public int? donGia { get; set; }
    public int? thanhTien { get; set; }
}


