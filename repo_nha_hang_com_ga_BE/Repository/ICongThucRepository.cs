using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.CongThuc;
using repo_nha_hang_com_ga_BE.Models.Responds.CongThuc;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ICongThucRepository
{
    Task<RespondAPIPaging<List<CongThucRespond>>> GetAllCongThucs(RequestSearchCongThuc request);
    Task<RespondAPI<CongThucRespond>> GetCongThucById(string id);
    Task<RespondAPI<CongThucRespond>> CreateCongThuc(RequestAddCongThuc product);
    Task<RespondAPI<CongThucRespond>> UpdateCongThuc(string id, RequestUpdateCongThuc product);
    Task<RespondAPI<string>> DeleteCongThuc(string id);
}