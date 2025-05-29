using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class LoaiTuDo : BaseMongoDb
{
    public string? tenLoai { get; set; }
    public string? moTa { get; set; }
}
