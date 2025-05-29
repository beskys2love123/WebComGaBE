using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.Ban;

public class RequestSearchBan : PagingParameterModel
{
    public string? tenBan { get; set; }

    public string? idLoaiBan { get; set; }

    public TrangThaiBan? trangThai { get; set; }
}