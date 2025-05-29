using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiBan;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiBan;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILoaiBanRepository
{
    Task<RespondAPIPaging<List<LoaiBanRespond>>> GetAllLoaiBans(RequestSearchLoaiBan request);
    Task<RespondAPI<LoaiBanRespond>> GetLoaiBanById(string id);
    Task<RespondAPI<LoaiBanRespond>> CreateLoaiBan(RequestAddLoaiBan product);
    Task<RespondAPI<LoaiBanRespond>> UpdateLoaiBan(string id, RequestUpdateLoaiBan product);
    Task<RespondAPI<string>> DeleteLoaiBan(string id);
}

