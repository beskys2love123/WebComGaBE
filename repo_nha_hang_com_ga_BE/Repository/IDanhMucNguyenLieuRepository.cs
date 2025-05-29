using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IDanhMucNguyenLieuRepository
{
    Task<RespondAPIPaging<List<DanhMucNguyenLieuRespond>>> GetAllDanhMucNguyenLieus(RequestSearchDanhMucNguyenLieu request);
    Task<RespondAPI<DanhMucNguyenLieuRespond>> GetDanhMucNguyenLieuById(string id);
    Task<RespondAPI<DanhMucNguyenLieuRespond>> CreateDanhMucNguyenLieu(RequestAddDanhMucNguyenLieu product);
    Task<RespondAPI<DanhMucNguyenLieuRespond>> UpdateDanhMucNguyenLieu(string id, RequestUpdateDanhMucNguyenLieu product);
    Task<RespondAPI<string>> DeleteDanhMucNguyenLieu(string id);
}