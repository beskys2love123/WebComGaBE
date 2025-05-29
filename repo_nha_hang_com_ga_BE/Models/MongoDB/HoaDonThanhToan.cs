
using System.ComponentModel;
using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;
public class HoaDonThanhToan : BaseMongoDb
{
    public string? nhanVien { get; set; }
    public string? donOrder { get; set; }
    public string? phuongThucThanhToan { get; set; }
    public string? nhaHang { get; set; }
    public string? tenHoaDon { get; set; }
    public string? qrCode { get; set; }
    public DateTimeOffset? gioVao { get; set; }
    public DateTimeOffset? gioRa { get; set; }
    public int? soNguoi { get; set; }
    public string? khuyenMai { get; set; }
    public string? phuPhi { get; set; }
    public TrangThaiHoaDon? trangthai { get; set; }

}

public enum TrangThaiHoaDon
{
    [Description("Chưa thanh toán")]
    ChuaThanhToan = 0,
    [Description("Đã thanh toán")]
    DaThanhToan = 1,
}