using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;

public class PhanQuyen : BaseMongoDb
{
    public string? tenPhanQuyen { get; set; }
    public string? moTa { get; set; }

    public List<string>? danhSachMenu { get; set; }
}