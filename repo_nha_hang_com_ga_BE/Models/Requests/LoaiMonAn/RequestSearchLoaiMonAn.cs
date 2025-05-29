using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.LoaiMonAn;

public class RequestSearchLoaiMonAn : PagingParameterModel
{
    public string? tenLoai { get; set; }

    public string? danhMucMonAnId { get; set; }
}