using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.GiamGia;
using repo_nha_hang_com_ga_BE.Models.Responds.GiamGia;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IGiamGiaRepository
{
    Task<RespondAPIPaging<List<GiamGiaRespond>>> GetAllGiamGias(RequestSearchGiamGia request);
    Task<RespondAPI<GiamGiaRespond>> GetGiamGiaById(string id);
    Task<RespondAPI<GiamGiaRespond>> CreateGiamGia(RequestAddGiamGia product);
    Task<RespondAPI<GiamGiaRespond>> UpdateGiamGia(string id, RequestUpdateGiamGia product);
    Task<RespondAPI<string>> DeleteGiamGia(string id);
}

