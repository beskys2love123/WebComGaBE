using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.GiamGia;
using repo_nha_hang_com_ga_BE.Models.Responds.GiamGia;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class GiamGiaRepository : IGiamGiaRepository
{
    private readonly IMongoCollection<GiamGia> _collection;
    private readonly IMapper _mapper;

    public GiamGiaRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<GiamGia>("GiamGia");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<GiamGiaRespond>>> GetAllGiamGias(RequestSearchGiamGia request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<GiamGia>.Filter.Empty;
            filter &= Builders<GiamGia>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenGiamGia))
            {
                filter &= Builders<GiamGia>.Filter.Regex(x => x.tenGiamGia, new BsonRegularExpression($".*{request.tenGiamGia}.*"));
            }

            if (request.ngayBatDau != null)
            {
                filter &= Builders<GiamGia>.Filter.Gte(x => x.ngayBatDau, request.ngayBatDau);
            }

            if (request.ngayKetThuc != null)
            {
                filter &= Builders<GiamGia>.Filter.Lte(x => x.ngayKetThuc, request.ngayKetThuc);
            }

            if (request.giaTri != null)
            {
                filter &= Builders<GiamGia>.Filter.Eq(x => x.giaTri, request.giaTri);
            }

            var projection = Builders<GiamGia>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenGiamGia)
                .Include(x => x.ngayBatDau)
                .Include(x => x.ngayKetThuc)
                .Include(x => x.giaTri)
                .Include(x => x.moTa);


            var findOptions = new FindOptions<GiamGia, GiamGiaRespond>
            {
                Projection = projection
            };

            if (request.IsPaging)
            {
                long totalRecords = await collection.CountDocumentsAsync(filter);

                int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                int currentPage = request.PageNumber;
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages) currentPage = totalPages;

                findOptions.Skip = (currentPage - 1) * request.PageSize;
                findOptions.Limit = request.PageSize;

                var cursor = await collection.FindAsync(filter, findOptions);
                var giamGias = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<GiamGiaRespond>>
                {
                    Paging = pagingDetail,
                    Data = giamGias
                };

                return new RespondAPIPaging<List<GiamGiaRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var giamGias = await cursor.ToListAsync();

                return new RespondAPIPaging<List<GiamGiaRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<GiamGiaRespond>>
                    {
                        Data = giamGias,
                        Paging = new PagingDetail(1, giamGias.Count, giamGias.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<GiamGiaRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<GiamGiaRespond>> GetGiamGiaById(string id)
    {
        try
        {
            var giamGia = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (giamGia == null)
            {
                return new RespondAPI<GiamGiaRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy giảm giá với ID đã cung cấp."
                );
            }

            var giamGiaRespond = _mapper.Map<GiamGiaRespond>(giamGia);

            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Succeeded,
                "Lấy giảm giá thành công.",
                giamGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<GiamGiaRespond>> CreateGiamGia(RequestAddGiamGia request)
    {
        try
        {
            GiamGia newGiamGia = _mapper.Map<GiamGia>(request);

            newGiamGia.createdDate = DateTimeOffset.UtcNow;
            newGiamGia.updatedDate = DateTimeOffset.UtcNow;
            newGiamGia.isDelete = false;

            await _collection.InsertOneAsync(newGiamGia);

            var giamGiaRespond = _mapper.Map<GiamGiaRespond>(newGiamGia);

            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Succeeded,
                "Tạo giảm giá thành công.",
                giamGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo giảm giá: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<GiamGiaRespond>> UpdateGiamGia(string id, RequestUpdateGiamGia request)
    {
        try
        {
            var filter = Builders<GiamGia>.Filter.Eq(x => x.Id, id);
            filter &= Builders<GiamGia>.Filter.Eq(x => x.isDelete, false);
            var giamGia = await _collection.Find(filter).FirstOrDefaultAsync();

            if (giamGia == null)
            {
                return new RespondAPI<GiamGiaRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy giảm giá với ID đã cung cấp."
                );
            }

            _mapper.Map(request, giamGia);

            giamGia.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, giamGia);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<GiamGiaRespond>(
                    ResultRespond.Error,
                    "Cập nhật giảm giá không thành công."
                );
            }

            var giamGiaRespond = _mapper.Map<GiamGiaRespond>(giamGia);

            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Succeeded,
                "Cập nhật giảm giá thành công.",
                giamGiaRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<GiamGiaRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật giảm giá: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteGiamGia(string id)
    {
        try
        {
            var existingGiamGia = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingGiamGia == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy giảm giá để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa giảm giá không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa giảm giá thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa giảm giá: {ex.Message}"
            );
        }
    }

}