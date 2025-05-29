using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiNguyenLieu;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILoaiNguyenLieuRepository
{
    Task<RespondAPIPaging<List<LoaiNguyenLieuRespond>>> GetAllLoaiNguyenLieus(RequestSearchLoaiNguyenLieu request);
    Task<RespondAPI<LoaiNguyenLieuRespond>> GetLoaiNguyenLieuById(string id);
    Task<RespondAPI<LoaiNguyenLieuRespond>> CreateLoaiNguyenLieu(RequestAddLoaiNguyenLieu product);
    Task<RespondAPI<LoaiNguyenLieuRespond>> UpdateLoaiNguyenLieu(string id, RequestUpdateLoaiNguyenLieu product);
    Task<RespondAPI<string>> DeleteLoaiNguyenLieu(string id);
}

