using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests;

public class RequestSearchNhaCungCap : PagingParameterModel
{
    public string? tenNhaCungCap { get; set; }
    public string? diaChi { get; set; }

}