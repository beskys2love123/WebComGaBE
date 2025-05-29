using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.MongoDB;

namespace repo_nha_hang_com_ga_BE.Models.Requests.DonOrder;
public class RequestUpdateStatusDonOrder
{
    public TrangThaiDonOrder? trangThai { get; set; }

}