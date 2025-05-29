using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using AutoMapper;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;

using MongoDB.Bson;

using Microsoft.Extensions.Options;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using System.ComponentModel;
using repo_nha_hang_com_ga_BE.Models.Responds.BangGia;
using repo_nha_hang_com_ga_BE.Models.Requests;
using repo_nha_hang_com_ga_BE.Models.Requests.BangGia;
using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class BangGiaRepository : IBangGiaRepository
{
    private readonly IMongoCollection<BangGia> _collection;
    private readonly IMapper _mapper;
    private readonly IMongoCollection<MonAn> _collectionMonAn;

    public BangGiaRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<BangGia>("BangGia");
        _collectionMonAn = database.GetCollection<MonAn>("MonAn");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<BangGiaRespond>>> GetAllBangGias(RequestSearchBangGia request)
    {
        try
        {
            var collection = _collection;
            var filter = Builders<BangGia>.Filter.Empty;
            filter &= Builders<BangGia>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenGia))
            {
                filter &= Builders<BangGia>.Filter.Regex(x => x.tenGia, new BsonRegularExpression($".*{request.tenGia}.*"));

            }

            if (!string.IsNullOrEmpty(request.idMonAn))
            {
                filter &= Builders<BangGia>.Filter.Eq(x => x.monAn, request.idMonAn);
            }


            var projection = Builders<BangGia>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenGia)
                .Include(x => x.monAn)
                .Include(x => x.createdDate)
                .Include(x => x.giaTri);

            var findOptions = new FindOptions<BangGia, BangGia>
            {
                Projection = projection
            };

            if (request.IsPaging)
            {
                long totalRecord = await collection.CountDocumentsAsync(filter);
                int totalPages = (int)Math.Ceiling((double)totalRecord / request.PageSize);

                int currentPage = request.PageNumber;
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages) currentPage = totalPages;

                findOptions.Skip = (currentPage - 1) * request.PageSize;
                findOptions.Limit = request.PageSize;

                var cursor = await collection.FindAsync(filter, findOptions);
                var BangGias = await cursor.ToListAsync();

                var monAnIds = BangGias.Select(x => x.monAn).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
                var monAnProjection = Builders<MonAn>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenMonAn);
                var monAns = await _collectionMonAn.Find(monAnFilter)
                    .Project<MonAn>(monAnProjection)
                    .ToListAsync();
                var monAnDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
                var BangGiaRespond = BangGias.Select(bangGia => new BangGiaRespond
                {
                    id = bangGia.Id,
                    tenGia = bangGia.tenGia,
                    giaTri = bangGia.giaTri,
                    createdDate = bangGia.createdDate?.Date,
                    monAn = new IdName
                    {
                        Id = bangGia.monAn,
                        Name = bangGia.monAn != null && monAnDict.ContainsKey(bangGia.monAn) ? monAnDict[bangGia.monAn] : null
                    }

                }).ToList();


                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecord);
                var pagingResponse = new PagingResponse<List<BangGiaRespond>>
                {
                    Paging = pagingDetail,
                    Data = BangGiaRespond
                };

                return new RespondAPIPaging<List<BangGiaRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var BangGias = await cursor.ToListAsync();

                var monAnIds = BangGias.Select(x => x.monAn).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
                var monAnProjection = Builders<MonAn>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenMonAn);
                var monAns = await _collectionMonAn.Find(monAnFilter)
                    .Project<MonAn>(monAnProjection)
                    .ToListAsync();
                var monAnDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
                var BangGiaRespond = BangGias.Select(bangGia => new BangGiaRespond
                {
                    id = bangGia.Id,
                    tenGia = bangGia.tenGia,
                    giaTri = bangGia.giaTri,
                    createdDate = bangGia.createdDate?.Date,
                    monAn = new IdName
                    {
                        Id = bangGia.monAn,
                        Name = bangGia.monAn != null && monAnDict.ContainsKey(bangGia.monAn) ? monAnDict[bangGia.monAn] : null
                    }

                }).ToList();

                return new RespondAPIPaging<List<BangGiaRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<BangGiaRespond>>
                    {
                        Data = BangGiaRespond,
                        Paging = new PagingDetail(1, BangGiaRespond.Count, BangGiaRespond.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<BangGiaRespond>>(
                ResultRespond.Failed,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<BangGiaRespond>> GetBangGiaById(string id)
    {
        try
        {
            var BangGia = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (BangGia == null)
            {
                return new RespondAPI<BangGiaRespond>(
                    ResultRespond.Failed,
                    message: "Bảng giá không tồn tại"
                );
            }
            var monAn = await _collectionMonAn.Find(x => x.Id == BangGia.monAn).FirstOrDefaultAsync();
            var BangGiaRespond = new BangGiaRespond
            {
                id = BangGia.Id,
                tenGia = BangGia.tenGia,
                giaTri = BangGia.giaTri,
                createdDate = BangGia.createdDate?.Date,
                monAn = new IdName
                {
                    Id = monAn.Id,
                    Name = monAn.tenMonAn
                }
            };

            return new RespondAPI<BangGiaRespond>(
                ResultRespond.Succeeded,
                "Lấy thông tin bảng giá thành công",
                BangGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<BangGiaRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<BangGiaRespond>> CreateBangGia(RequestAddBangGia request)
    {
        try
        {
            BangGia newBangGia = _mapper.Map<BangGia>(request);

            newBangGia.isDelete = false;
            newBangGia.createdDate = DateTimeOffset.UtcNow;
            newBangGia.updatedDate = DateTimeOffset.UtcNow;

            await _collection.InsertOneAsync(newBangGia);
            var monAn = await _collectionMonAn.Find(x => x.Id == newBangGia.monAn).FirstOrDefaultAsync();
            var BangGiaRespond = new BangGiaRespond
            {
                id = newBangGia.Id,
                tenGia = newBangGia.tenGia,
                giaTri = newBangGia.giaTri,
                createdDate = newBangGia.createdDate?.Date,
                monAn = new IdName
                {
                    Id = monAn.Id,
                    Name = monAn.tenMonAn
                }
            };



            return new RespondAPI<BangGiaRespond>(
                ResultRespond.Succeeded,
                "Thêm bảng giá thành công",
                BangGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<BangGiaRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<BangGiaRespond>> UpdateBangGia(string id, RequestUpdateBangGia request)
    {
        try
        {
            var filter = Builders<BangGia>.Filter.Eq(x => x.Id, id);
            filter &= Builders<BangGia>.Filter.Eq(x => x.isDelete, false);
            var BangGia = await _collection.Find(filter).FirstOrDefaultAsync();

            if (BangGia == null)
            {
                return new RespondAPI<BangGiaRespond>(
                    ResultRespond.NotFound,
                    "Bảng giá không tồn tại."
                );
            }

            _mapper.Map(request, BangGia);

            BangGia.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, BangGia);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<BangGiaRespond>(
                    ResultRespond.Error,
                    "Cập nhật Bảng giá không thành công."
                );
            }

            var monAn = await _collectionMonAn.Find(x => x.Id == BangGia.monAn).FirstOrDefaultAsync();
            var BangGiaRespond = new BangGiaRespond
            {
                id = BangGia.Id,
                tenGia = BangGia.tenGia,
                giaTri = BangGia.giaTri,
                createdDate = BangGia.createdDate?.Date,
                monAn = new IdName
                {
                    Id = monAn.Id,
                    Name = monAn.tenMonAn
                }
            };

            return new RespondAPI<BangGiaRespond>(
                ResultRespond.Succeeded,
                "Cập nhật Bảng giá thành công",
                BangGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<BangGiaRespond>
            (
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật thông tin Bảng giá: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteBangGia(string id)
    {
        try
        {
            var existingBangGia = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (existingBangGia == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    message: "Bảng giá không tồn tại"
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa Bảng giá không thành công."
                );
            }
            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa Bảng giá thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa Bảng giá: {ex.Message}"
            );
        }
    }

}