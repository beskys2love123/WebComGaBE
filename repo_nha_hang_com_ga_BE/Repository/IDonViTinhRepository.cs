using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Responds.DonViTinh;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IDonViTinhRepository
{
    Task<RespondAPIPaging<List<DonViTinhRespond>>> GetAllDonViTinhs(RequestSearchDonViTinh request);
    Task<RespondAPI<DonViTinhRespond>> GetDonViTinhById(string id);
    Task<RespondAPI<DonViTinhRespond>> CreateDonViTinh(RequestAddDonViTinh product);
    Task<RespondAPI<DonViTinhRespond>> UpdateDonViTinh(string id, RequestUpdateDonViTinh product);
    Task<RespondAPI<string>> DeleteDonViTinh(string id);
}

