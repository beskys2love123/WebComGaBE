using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class LoaiBan : BaseMongoDb
{
    public string? tenLoai { get; set; }
    public string? moTa { get; set; }
}

