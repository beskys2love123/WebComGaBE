using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Requests.BangGia;

public class RequestUpdateBangGia
{
    public string? tenGia { get; set; }

    public int? giaTri { get; set; }
    public string? monAn {get; set;}
}