using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.HoaDonThanhToan;

public class RequestSearchHoaDonThanhToan : PagingParameterModel
{
    public string? nhanVien { get; set; }
    public string? donOrder { get; set; }
    public string? phuongThucThanhToan { get; set; }
    public string? tenHoaDon { get; set; }
    public DateTimeOffset? gioVao { get; set; }
    public DateTimeOffset? gioRa { get; set; }
    public int? soNguoi { get; set; }
    public TrangThaiHoaDon? trangthai { get; set; }
}