using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.Common;

public class LoaiNguyenLieuCongThucRespond : IdName
{
    public List<NguyenLieuCongThucRespond>? nguyenLieus { get; set; }
    public string? ghiChu { get; set; }
}
