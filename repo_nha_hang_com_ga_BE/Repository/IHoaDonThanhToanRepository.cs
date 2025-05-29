using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.BaoCaoThongKe;
using repo_nha_hang_com_ga_BE.Models.Requests.HoaDonThanhToan;
using repo_nha_hang_com_ga_BE.Models.Responds.HoaDonThanhToan;


namespace repo_nha_hang_com_ga_BE.Repository;

public interface IHoaDonThanhToanRepository
{
    Task<RespondAPIPaging<List<HoaDonThanhToanRespond>>> GetAllHoaDonThanhToan(RequestSearchHoaDonThanhToan request);
    Task<RespondAPI<HoaDonThanhToanRespond>> GetHoaDonThanhToanById(string id);
    Task<RespondAPI<HoaDonThanhToanRespond>> CreateHoaDonThanhToan(RequestAddHoaDonThanhToan request);
    Task<RespondAPI<HoaDonThanhToanRespond>> UpdateHoaDonThanhToan(string id, RequestUpdateHoaDonThanhToan request);
    Task<RespondAPI<string>> DeleteHoaDonThanhToan(string id);
    Task<List<DoanhThuMonAnRespond>> GetDoanhThu(RequestSearchThoiGian request);
    Task<List<BestSellingMonAnRespond>> GetBestSellingMonAn(RequestSearchThoiGian request);
    Task<List<MatDoKhachHangRespond>> GetMatDoKhachHang(RequestSearchThoiGian request);
}

public class DoanhThuMonAnRespond
{
    public string? thoiGian { get; set; }
    public int? doanhThu { get; set; }
}

public class BestSellingMonAnRespond
{
    public string? monAn { get; set; }
    public int? soLuong { get; set; }
}

public class MatDoKhachHangRespond
{
    public string? thoiGian { get; set; }
    public int? matDoKhachHang { get; set; }
}


