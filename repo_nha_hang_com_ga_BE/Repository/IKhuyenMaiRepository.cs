using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.KhuyenMai;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IKhuyenMaiRepository
{
    Task<RespondAPIPaging<List<KhuyenMaiRespond>>> GetAllKhuyenMais(RequestSearchKhuyenMai request);
    Task<RespondAPI<KhuyenMaiRespond>> GetKhuyenMaiById(string id);
    Task<RespondAPI<KhuyenMaiRespond>> CreateKhuyenMai(RequestAddKhuyenMai product);
    Task<RespondAPI<KhuyenMaiRespond>> UpdateKhuyenMai(string id, RequestUpdateKhuyenMai product);
    Task<RespondAPI<string>> DeleteKhuyenMai(string id);
}

