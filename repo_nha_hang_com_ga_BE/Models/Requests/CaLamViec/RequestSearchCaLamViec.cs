using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
namespace repo_nha_hang_com_ga_BE.Models.Requests.CaLamViec;
public class RequestSearchCaLamViec : PagingParameterModel
{
    public string? tenCaLamViec { get; set; }
    public string? khungThoiGian { get; set; }
    // public string? moTa { get; set; }
}