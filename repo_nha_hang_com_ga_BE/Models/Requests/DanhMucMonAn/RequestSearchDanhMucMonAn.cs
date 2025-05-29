using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DanhMucMonAn;

public class RequestSearchDanhMucMonAn : PagingParameterModel
{
    public string? tenDanhMuc { get; set; }
}