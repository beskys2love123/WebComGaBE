using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;

public class RequestAddKhuyenMai
{
    public string? tenKhuyenMai { get; set; }
    public DateTimeOffset ngayBatDau { get; set; }
    public DateTimeOffset ngayKetThuc { get; set; }
    public double giaTri { get; set; }
}