using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.MenuDynamic;
using repo_nha_hang_com_ga_BE.Models.Responds.MenuDynamic;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IMenuDynamicRepository
{
    Task<RespondAPIPaging<List<MenuDynamicRespond>>> GetAllMenuDynamics(RequestSearchMenuDynamic request);
    Task<RespondAPI<MenuDynamicRespond>> GetMenuDynamicById(string id);
    Task<RespondAPI<MenuDynamicRespond>> CreateMenuDynamic(RequestAddMenuDynamic product);
    Task<RespondAPI<MenuDynamicRespond>> UpdateMenuDynamic(string id, RequestUpdateMenuDynamic product);
    Task<RespondAPI<string>> DeleteMenuDynamic(string id);

    Task<RespondAPIPaging<List<MenuDynamicRespond>>> GetAllMenuDynamicsByPhanQuyen(List<string> danhSachPhanQuyen);
}