using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.KhachHang;
using repo_nha_hang_com_ga_BE.Models.Responds.KhachHang;

namespace repo_nha_hang_com_ga_BE.Repository;

public interface IKhachHangRepository
{
    Task<RespondAPIPaging<List<KhachHangRespond>>> GetAllKhachHangs(RequestSearchKhachHang request);
    Task<RespondAPI<KhachHangRespond>> GetKhachHangById(string id);
    Task<RespondAPI<KhachHangRespond>> CreateKhachHang(RequestAddKhachHang product);
    Task<RespondAPI<KhachHangRespond>> UpdateKhachHang(string id, RequestUpdateKhachHang product);
    Task<RespondAPI<string>> DeleteKhachHang(string id);
}