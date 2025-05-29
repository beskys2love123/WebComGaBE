using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class CongThuc : BaseMongoDb
{
    public string? tenCongThuc { get; set; }
    public List<LoaiNguyenLieuCongThuc>? loaiNguyenLieus { get; set; }
    public string? moTa { get; set; }
    public string? hinhAnh { get; set; }
}
