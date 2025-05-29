using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucMonAn;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IDanhMucMonAnRepository
{
    Task<RespondAPIPaging<List<DanhMucMonAnRespond>>> GetAllDanhMucMonAns(RequestSearchDanhMucMonAn request);
    Task<RespondAPI<DanhMucMonAnRespond>> GetDanhMucMonAnById(string id);
    Task<RespondAPI<DanhMucMonAnRespond>> CreateDanhMucMonAn(RequestAddDanhMucMonAn product);
    Task<RespondAPI<DanhMucMonAnRespond>> UpdateDanhMucMonAn(string id, RequestUpdateDanhMucMonAn product);
    Task<RespondAPI<string>> DeleteDanhMucMonAn(string id);
}