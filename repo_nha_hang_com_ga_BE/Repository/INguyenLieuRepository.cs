using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.NguyenLieu;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface INguyenLieuRepository
{
    Task<RespondAPIPaging<List<NguyenLieuRespond>>> GetAllNguyenLieus(RequestSearchNguyenLieu request);
    Task<RespondAPI<NguyenLieuRespond>> GetNguyenLieuById(string id);
    Task<RespondAPI<NguyenLieuRespond>> CreateNguyenLieu(RequestAddNguyenLieu product);
    Task<RespondAPI<NguyenLieuRespond>> UpdateNguyenLieu(string id, RequestUpdateNguyenLieu product);
    Task<RespondAPI<string>> DeleteNguyenLieu(string id);
    Task<RespondAPI<List<NguyenLieuRespond>>> CreateListNguyenLieu(RequestAddListNguyenLieu request);
}

