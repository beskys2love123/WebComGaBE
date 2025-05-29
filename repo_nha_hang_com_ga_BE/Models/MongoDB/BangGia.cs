using repo_nha_hang_com_ga_BE.Models.Common;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB;
public class BangGia : BaseMongoDb
{
    public string? tenGia { get; set; }
    public string? monAn{get; set;}
    public int? giaTri { get; set; }

}