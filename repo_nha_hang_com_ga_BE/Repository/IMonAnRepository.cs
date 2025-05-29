using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.MonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.MonAn;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IMonAnRepository
{
    Task<RespondAPIPaging<List<MonAnRespond>>> GetAllMonAns(RequestSearchMonAn request);
    Task<RespondAPI<MonAnRespond>> GetMonAnById(string id);
    Task<RespondAPI<MonAnRespond>> CreateMonAn(RequestAddMonAn product);
    Task<RespondAPI<MonAnRespond>> UpdateMonAn(string id, RequestUpdateMonAn product);
    Task<RespondAPI<string>> DeleteMonAn(string id);
}