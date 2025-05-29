using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests;
using repo_nha_hang_com_ga_BE.Models.Requests.BangGia;
using repo_nha_hang_com_ga_BE.Models.Responds;
using repo_nha_hang_com_ga_BE.Models.Responds.Ban;
using repo_nha_hang_com_ga_BE.Models.Responds.BangGia;



namespace repo_nha_hang_com_ga_BE.Repository;

public interface IBangGiaRepository
{
    Task<RespondAPIPaging<List<BangGiaRespond>>> GetAllBangGias(RequestSearchBangGia request);
    Task<RespondAPI<BangGiaRespond>> GetBangGiaById(string id);
    Task<RespondAPI<BangGiaRespond>> CreateBangGia(RequestAddBangGia product);
    Task<RespondAPI<BangGiaRespond>> UpdateBangGia(string id, RequestUpdateBangGia product);
    Task<RespondAPI<string>> DeleteBangGia(string id);
}