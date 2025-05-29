using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.TuDo;

public class RequestSearchTuDo : PagingParameterModel
{
    public string? tenTuDo { get; set; }

    public string? loaiTuDoId { get; set; }

}