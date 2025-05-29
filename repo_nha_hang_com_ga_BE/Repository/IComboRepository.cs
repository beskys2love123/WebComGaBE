using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.Combo;
using repo_nha_hang_com_ga_BE.Models.Responds.Combo;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IComboRepository
{
    Task<RespondAPIPaging<List<ComboRespond>>> GetAllCombos(RequestSearchCombo request);
    Task<RespondAPI<ComboRespond>> GetComboById(string id);
    Task<RespondAPI<ComboRespond>> CreateCombo(RequestAddCombo product);
    Task<RespondAPI<ComboRespond>> UpdateCombo(string id, RequestUpdateCombo product);
    Task<RespondAPI<string>> DeleteCombo(string id);
}