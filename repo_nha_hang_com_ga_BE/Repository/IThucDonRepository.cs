using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.ThucDon;
using repo_nha_hang_com_ga_BE.Models.Responds.ThucDon;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IThucDonRepository
{
    Task<RespondAPIPaging<List<ThucDonRespond>>> GetAllThucDons(RequestSearchThucDon request);
    Task<RespondAPI<ThucDonRespond>> GetThucDonById(string id);
    Task<RespondAPI<ThucDonRespond>> CreateThucDon(RequestAddThucDon product);
    Task<RespondAPI<ThucDonRespond>> UpdateThucDon(string id, RequestUpdateThucDon product);
    Task<RespondAPI<string>> DeleteThucDon(string id);
}