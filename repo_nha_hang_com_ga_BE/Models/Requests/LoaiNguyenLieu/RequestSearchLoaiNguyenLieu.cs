using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;

public class RequestSearchLoaiNguyenLieu : PagingParameterModel
{
    public string? tenLoai { get; set; }

    public string? danhMucNguyenLieuId { get; set; }
}