using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.TuDo;
using repo_nha_hang_com_ga_BE.Models.Responds.TuDo;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ITuDoRepository
{
    Task<RespondAPIPaging<List<TuDoRespond>>> GetAllTuDos(RequestSearchTuDo request);
    Task<RespondAPI<TuDoRespond>> GetTuDoById(string id);
    Task<RespondAPI<TuDoRespond>> CreateTuDo(RequestAddTuDo product);
    Task<RespondAPI<TuDoRespond>> UpdateTuDo(string id, RequestUpdateTuDo product);
    Task<RespondAPI<string>> DeleteTuDo(string id);
}

