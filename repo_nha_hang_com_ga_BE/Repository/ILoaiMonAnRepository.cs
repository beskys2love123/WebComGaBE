using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiMonAn;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILoaiMonAnRepository
{
    Task<RespondAPIPaging<List<LoaiMonAnRespond>>> GetAllLoaiMonAns(RequestSearchLoaiMonAn request);
    Task<RespondAPI<LoaiMonAnRespond>> GetLoaiMonAnById(string id);
    Task<RespondAPI<LoaiMonAnRespond>> CreateLoaiMonAn(RequestAddLoaiMonAn product);
    Task<RespondAPI<LoaiMonAnRespond>> UpdateLoaiMonAn(string id, RequestUpdateLoaiMonAn product);
    Task<RespondAPI<string>> DeleteLoaiMonAn(string id);
}

