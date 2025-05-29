using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using System.ComponentModel;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB
{
    public class KhachHang : BaseMongoDb
    {
        public string? tenKhachHang { get; set; }
        public string? diaChi { get; set; }
        public string? email { get; set; }
        public string? soDienThoai { get; set; }
    }
}