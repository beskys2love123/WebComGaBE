using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.LoaiKhuyenMai;

public class RequestSearchLoaiKhuyenMai : PagingParameterModel
{
    public string? tenLoai { get; set; }

}