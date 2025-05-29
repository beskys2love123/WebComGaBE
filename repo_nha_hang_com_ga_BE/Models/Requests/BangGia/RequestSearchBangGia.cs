using repo_nha_hang_com_ga_BE.Models.Common.Models.Request;

namespace repo_nha_hang_com_ga_BE.Models.Requests.BangGia;

public class RequestSearchBangGia : PagingParameterModel
{
    public string? tenGia { get; set; }
    public string? idMonAn {get; set;}


}