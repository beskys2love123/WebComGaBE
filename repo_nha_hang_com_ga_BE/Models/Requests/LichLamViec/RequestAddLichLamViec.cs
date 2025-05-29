namespace repo_nha_hang_com_ga_BE.Models.Requests.LichLamViec;
public class RequestAddLichLamViec
{
    public DateTime? ngay { get; set; }
    public List<ChiTietLichLamViec>? chiTietLichLamViec { get; set; }
    public string? moTa { get; set; }
}
