
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.LichLamViecRespond;
public class LichLamViecRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    public DateTime? ngay { get; set; }
    public List<ChiTietLichLamViecRespond>? chiTietLichLamViec { get; set; }
    public string? moTa { get; set; }

}

public class ChiTietLichLamViecRespond
{
    public IdName? caLamViec { get; set; }
    public List<NhanVienCaRespond>? nhanVienCa { get; set; }
    public string? moTa { get; set; }
}

public class NhanVienCaRespond
{
    public IdName? nhanVien { get; set; }
    public string? moTa { get; set; }
}