using repo_nha_hang_com_ga_BE.Models.Common.Models;
using System.ComponentModel;
namespace repo_nha_hang_com_ga_BE.Models.Requests.BaoCaoThongKe;

public class RequestSearchThoiGian
{
    public DoanhThuEnum? doanhThuEnum { get; set; }
    public DateTime? tuNgay { get; set; }
    public DateTime? denNgay { get; set; }
    public int? soTuan { get; set; }
}

public enum DoanhThuEnum
{
    [Description("Theo Ngày")]
    TheoNgay = 0,
    [Description("Theo Tuần")]
    TheoTuan = 1,
    [Description("Theo Tháng")]
    TheoThang = 2,
    [Description("Theo Năm")]
    TheoNam = 3,
}

