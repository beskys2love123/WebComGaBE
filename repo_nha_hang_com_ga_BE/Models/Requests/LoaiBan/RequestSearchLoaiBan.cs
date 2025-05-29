using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.LoaiBan;

public class RequestSearchLoaiBan : PagingParameterModel
{
    public string? tenLoai { get; set; }
}