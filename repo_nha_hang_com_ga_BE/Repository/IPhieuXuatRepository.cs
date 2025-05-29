using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuXuat;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuXuat;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IPhieuXuatRepository
{
    Task<RespondAPIPaging<List<PhieuXuatRespond>>> GetAllPhieuXuats(RequestSearchPhieuXuat request);
    Task<RespondAPI<PhieuXuatRespond>> GetPhieuXuatById(string id);
    Task<RespondAPI<PhieuXuatRespond>> CreatePhieuXuat(RequestAddPhieuXuat product);
    Task<RespondAPI<string>> DeletePhieuXuat(string id);
}