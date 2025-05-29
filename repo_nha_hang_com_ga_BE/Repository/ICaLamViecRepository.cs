using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.CaLamViec;
using repo_nha_hang_com_ga_BE.Models.Responds.CaLamViecRespond;

namespace repo_nha_hang_com_ga_BE.Models.Repositories;
public interface ICaLamViecRepository
{
    Task<RespondAPIPaging<List<CaLamViecRespond>>> GetAllCaLamViec(RequestSearchCaLamViec request);
    Task<RespondAPI<CaLamViecRespond>> GetCaLamViecById(string id);

    Task<RespondAPI<CaLamViecRespond>> CreateCaLamViec(RequestAddCaLamViec request);
    Task<RespondAPI<CaLamViecRespond>> UpdateCaLamViec(string id, RequestUpdateCaLamViec request);
    Task<RespondAPI<string>> DeleteCaLamViec(string id);


}