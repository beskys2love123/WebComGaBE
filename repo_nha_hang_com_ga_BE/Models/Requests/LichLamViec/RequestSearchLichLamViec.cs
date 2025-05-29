using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
namespace repo_nha_hang_com_ga_BE.Models.Requests.LichLamViec;
public class RequestSearchLichLamViec : PagingParameterModel
{
    public DateTime? ngay { get; set; }

    // public string? moTa { get; set; }
}