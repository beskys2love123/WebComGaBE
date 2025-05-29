using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using System.ComponentModel;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB
{
    public class LichLamViec : BaseMongoDb
    {
        public DateTime? ngay { get; set; }
        public List<ChiTietLichLamViec>? chiTietLichLamViec { get; set; }
        public string? moTa { get; set; }

    }
}

public class ChiTietLichLamViec
{
    public string? caLamViec { get; set; }
    public List<NhanVienCa>? nhanVienCa { get; set; }
    public string? moTa { get; set; }
}

public class NhanVienCa
{
    public string? nhanVien { get; set; }
    public string? moTa { get; set; }
}
