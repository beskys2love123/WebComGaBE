using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.DonOrder;
using repo_nha_hang_com_ga_BE.Models.Responds.DonOrder;


namespace repo_nha_hang_com_ga_BE.Repository;

public interface IDonOrderRepository
{
    Task<RespondAPIPaging<List<DonOrderRespond>>> GetAllDonOrder(RequestSearchDonOrder request);
    Task<RespondAPI<DonOrderRespond>> GetDonOrderById(string id);
    Task<RespondAPI<DonOrderRespond>> CreateDonOrder(RequestAddDonOrder request);
    Task<RespondAPI<DonOrderRespond>> UpdateDonOrder(string id, RequestUpdateDonOrder request);
    Task<RespondAPI<DonOrderRespond>> UpdateStatusDonOrder(string id, RequestUpdateStatusDonOrder request);
    Task<RespondAPI<string>> DeleteDonOrder(string id);

}