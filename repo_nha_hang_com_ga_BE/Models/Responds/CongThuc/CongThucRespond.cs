using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Responds.Common;

namespace repo_nha_hang_com_ga_BE.Models.Responds.CongThuc;

public class CongThucRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public string? tenCongThuc { get; set; }

    public List<LoaiNguyenLieuCongThucRespond>? loaiNguyenLieus { get; set; }

    public string? moTa { get; set; }

    public string? hinhAnh { get; set; }
}