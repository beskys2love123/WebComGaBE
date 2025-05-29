using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests;
using repo_nha_hang_com_ga_BE.Models.Responds;
using repo_nha_hang_com_ga_BE.Models.Responds.PhanQuyen;



namespace repo_nha_hang_com_ga_BE.Repository;

public interface IPhanQuyenRepository
{
    Task<RespondAPIPaging<List<PhanQuyenRespond>>> GetAllPhanQuyens(RequestSearchPhanQuyen request);
    Task<RespondAPI<PhanQuyenRespond>> GetPhanQuyenById(string id);
    Task<RespondAPI<PhanQuyenRespond>> CreatePhanQuyen(RequestAddPhanQuyen product);
    Task<RespondAPI<PhanQuyenRespond>> UpdatePhanQuyen(string id, RequestUpdatePhanQuyen product);
    Task<RespondAPI<string>> DeletePhanQuyen(string id);
}