using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Responds.DonDatBan;

public class DonDatBanRespond
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    public IdName? ban { get; set; }

    public IdName? khachHang { get; set; }

    public string? khungGio { get; set; }

}