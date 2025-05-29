using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using System.ComponentModel;

namespace repo_nha_hang_com_ga_BE.Models.MongoDB
{
    public class CaLamViec : BaseMongoDb
    {
        public string? tenCaLamViec { get; set; }
        public string? khungThoiGian { get; set; }
        public string? moTa { get; set; }

    }
}
