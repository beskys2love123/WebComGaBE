using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;

public class RequestSearchDanhMucNguyenLieu : PagingParameterModel
{
    public string? tenDanhMuc { get; set; }
}