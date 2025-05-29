using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.CongThuc;

public class RequestSearchCongThuc : PagingParameterModel
{
    public string? tenCongThuc { get; set; }
}