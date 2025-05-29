using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.Ban;
using repo_nha_hang_com_ga_BE.Models.Responds.Ban;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IBanRepository
{
    Task<RespondAPIPaging<List<BanRespond>>> GetAllBans(RequestSearchBan request);
    Task<RespondAPI<BanRespond>> GetBanById(string id);
    Task<RespondAPI<BanRespond>> CreateBan(RequestAddBan product);
    Task<RespondAPI<BanRespond>> UpdateBan(string id, RequestUpdateBan product);
    Task<RespondAPI<string>> DeleteBan(string id);
}