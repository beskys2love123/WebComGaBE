using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests;

public class RequestSearchPhanQuyen : PagingParameterModel
{
    public string? tenPhanQuyen { get; set; }

}