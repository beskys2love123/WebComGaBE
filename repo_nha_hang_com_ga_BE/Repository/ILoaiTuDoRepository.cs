using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiTuDo;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiTuDo;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILoaiTuDoRepository
{
    Task<RespondAPIPaging<List<LoaiTuDoRespond>>> GetAllLoaiTuDos(RequestSearchLoaiTuDo request);
    Task<RespondAPI<LoaiTuDoRespond>> GetLoaiTuDoById(string id);
    Task<RespondAPI<LoaiTuDoRespond>> CreateLoaiTuDo(RequestAddLoaiTuDo product);
    Task<RespondAPI<LoaiTuDoRespond>> UpdateLoaiTuDo(string id, RequestUpdateLoaiTuDo product);
    Task<RespondAPI<string>> DeleteLoaiTuDo(string id);
}

