using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiKhuyenMai;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface ILoaiKhuyenMaiRepository
{
    Task<RespondAPIPaging<List<LoaiKhuyenMaiRespond>>> GetAllLoaiKhuyenMais(RequestSearchLoaiKhuyenMai request);
    Task<RespondAPI<LoaiKhuyenMaiRespond>> GetLoaiKhuyenMaiById(string id);
    Task<RespondAPI<LoaiKhuyenMaiRespond>> CreateLoaiKhuyenMai(RequestAddLoaiKhuyenMai product);
    Task<RespondAPI<LoaiKhuyenMaiRespond>> UpdateLoaiKhuyenMai(string id, RequestUpdateLoaiKhuyenMai product);
    Task<RespondAPI<string>> DeleteLoaiKhuyenMai(string id);
}

