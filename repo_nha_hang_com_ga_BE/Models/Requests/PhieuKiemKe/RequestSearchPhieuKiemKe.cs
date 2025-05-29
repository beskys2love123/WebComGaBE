using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.PhieuKiemKe;

public class RequestSearchPhieuKiemKe: PagingParameterModel
{
    public string? tenPhieu { get; set; }
    public DateTime? tuNgay { get; set; }
    public DateTime? denNgay { get; set; }

}