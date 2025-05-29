using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LichLamViec;
using repo_nha_hang_com_ga_BE.Models.Responds.LichLamViecRespond;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILichLamViecRepository
{
    Task<RespondAPIPaging<List<LichLamViecRespond>>> GetAllLichLamViecs(RequestSearchLichLamViec request);
    Task<RespondAPI<LichLamViecRespond>> GetLichLamViecById(string id);
    Task<RespondAPI<LichLamViecRespond>> CreateLichLamViec(RequestAddLichLamViec product);
    Task<RespondAPI<LichLamViecRespond>> UpdateLichLamViec(string id, RequestUpdateLichLamViec product);
    Task<RespondAPI<string>> DeleteLichLamViec(string id);
}

