using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuKiemKe;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuKiemKe;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IPhieuKiemKeRepository
{
    Task<RespondAPIPaging<List<PhieuKiemKeRespond>>> GetAllPhieuKiemKes(RequestSearchPhieuKiemKe request);
    Task<RespondAPI<PhieuKiemKeRespond>> GetPhieuKiemKeById(string id);
    Task<RespondAPI<PhieuKiemKeRespond>> CreatePhieuKiemKe(RequestAddPhieuKiemKe product);
    Task<RespondAPI<string>> DeletePhieuKiemKe(string id);
}