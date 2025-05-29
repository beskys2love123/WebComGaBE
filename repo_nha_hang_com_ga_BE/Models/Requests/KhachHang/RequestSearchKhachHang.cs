using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
namespace repo_nha_hang_com_ga_BE.Models.Requests.KhachHang;

public class RequestSearchKhachHang : PagingParameterModel
{
    public string? tenKhachHang { get; set; }
    public string? diaChi { get; set; }
    public string? email { get; set; }
    public string? soDienThoai { get; set; }


}