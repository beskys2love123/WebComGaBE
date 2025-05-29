using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuThanhLy;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuThanhLy;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IPhieuThanhLyRepository
{
    Task<RespondAPIPaging<List<PhieuThanhLyRespond>>> GetAllPhieuThanhLys(RequestSearchPhieuThanhLy request);
    Task<RespondAPI<PhieuThanhLyRespond>> GetPhieuThanhLyById(string id);
    Task<RespondAPI<PhieuThanhLyRespond>> CreatePhieuThanhLy(RequestAddPhieuThanhLy product);
    Task<RespondAPI<string>> DeletePhieuThanhLy(string id);
}