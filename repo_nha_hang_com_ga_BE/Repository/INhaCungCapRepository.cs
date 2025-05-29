using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests;
using repo_nha_hang_com_ga_BE.Models.Responds;
using repo_nha_hang_com_ga_BE.Models.Responds.NhaCungCap;



namespace repo_nha_hang_com_ga_BE.Repository;

public interface INhaCungCapRepository
{
    Task<RespondAPIPaging<List<NhaCungCapRespond>>> GetAllNhaCungCaps(RequestSearchNhaCungCap request);
    Task<RespondAPI<NhaCungCapRespond>> GetNhaCungCapById(string id);
    Task<RespondAPI<NhaCungCapRespond>> CreateNhaCungCap(RequestAddNhaCungCap product);
    Task<RespondAPI<NhaCungCapRespond>> UpdateNhaCungCap(string id, RequestUpdateNhaCungCap product);
    Task<RespondAPI<string>> DeleteNhaCungCap(string id);
}