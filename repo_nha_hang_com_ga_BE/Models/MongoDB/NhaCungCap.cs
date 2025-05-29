using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;
public class NhaCungCap : BaseMongoDb
{

    public string? tenNhaCungCap { get; set; }

    public string? soDienThoai { get; set; }

    public string? diaChi { get; set; }
}