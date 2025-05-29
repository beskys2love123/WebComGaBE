using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.DonDatBan;
using repo_nha_hang_com_ga_BE.Models.Responds.DonDatBan;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IDonDatBanRepository
{
    Task<RespondAPIPaging<List<DonDatBanRespond>>> GetAllDonDatBan(RequestSearchDonDatBan request);
    Task<RespondAPI<DonDatBanRespond>> GetDonDatBanById(string id);
    Task<RespondAPI<DonDatBanRespond>> CreateDonDatBan(RequestAddDonDatBan request);
    Task<RespondAPI<DonDatBanRespond>> UpdateDonDatBan(string id, RequestUpdateDonDatBan request);
    Task<RespondAPI<string>> DeleteDonDatBan(string id);

}