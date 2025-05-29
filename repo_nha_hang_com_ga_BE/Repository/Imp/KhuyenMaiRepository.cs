using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.KhuyenMai;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class KhuyenMaiRepository : IKhuyenMaiRepository
{
    private readonly IMongoCollection<KhuyenMai> _collection;
    private readonly IMapper _mapper;

    public KhuyenMaiRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<KhuyenMai>("KhuyenMai");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<KhuyenMaiRespond>>> GetAllKhuyenMais(RequestSearchKhuyenMai request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<KhuyenMai>.Filter.Empty;
            filter &= Builders<KhuyenMai>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenKhuyenMai))
            {
                filter &= Builders<KhuyenMai>.Filter.Regex(x => x.tenKhuyenMai, new BsonRegularExpression($".*{request.tenKhuyenMai}.*"));
            }

            if (request.ngayBatDau != null)
            {
                filter &= Builders<KhuyenMai>.Filter.Gte(x => x.ngayBatDau, request.ngayBatDau);
            }

            if (request.ngayKetThuc != null)
            {
                filter &= Builders<KhuyenMai>.Filter.Lte(x => x.ngayKetThuc, request.ngayKetThuc);
            }

            if (request.giaTri != null)
            {
                filter &= Builders<KhuyenMai>.Filter.Eq(x => x.giaTri, request.giaTri);
            }

            var projection = Builders<KhuyenMai>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenKhuyenMai)
                .Include(x => x.ngayBatDau)
                .Include(x => x.ngayKetThuc)
                .Include(x => x.giaTri);

            var findOptions = new FindOptions<KhuyenMai, KhuyenMaiRespond>
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
                var khuyenMais = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<KhuyenMaiRespond>>
                {
                    Paging = pagingDetail,
                    Data = khuyenMais
                };

                return new RespondAPIPaging<List<KhuyenMaiRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var khuyenMais = await cursor.ToListAsync();

                return new RespondAPIPaging<List<KhuyenMaiRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<KhuyenMaiRespond>>
                    {
                        Data = khuyenMais,
                        Paging = new PagingDetail(1, khuyenMais.Count, khuyenMais.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<KhuyenMaiRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<KhuyenMaiRespond>> GetKhuyenMaiById(string id)
    {
        try
        {
            var khuyenMai = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (khuyenMai == null)
            {
                return new RespondAPI<KhuyenMaiRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy khuyến mãi với ID đã cung cấp."
                );
            }

            var khuyenMaiRespond = _mapper.Map<KhuyenMaiRespond>(khuyenMai);

            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Lấy khuyến mãi thành công.",
                khuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<KhuyenMaiRespond>> CreateKhuyenMai(RequestAddKhuyenMai request)
    {
        try
        {
            KhuyenMai newKhuyenMai = _mapper.Map<KhuyenMai>(request);

            newKhuyenMai.createdDate = DateTimeOffset.UtcNow;
            newKhuyenMai.updatedDate = DateTimeOffset.UtcNow;
            newKhuyenMai.isDelete = false;

            await _collection.InsertOneAsync(newKhuyenMai);

            var khuyenMaiRespond = _mapper.Map<KhuyenMaiRespond>(newKhuyenMai);

            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Tạo khuyến mãi thành công.",
                khuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo khuyến mãi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<KhuyenMaiRespond>> UpdateKhuyenMai(string id, RequestUpdateKhuyenMai request)
    {
        try
        {
            var filter = Builders<KhuyenMai>.Filter.Eq(x => x.Id, id);
            filter &= Builders<KhuyenMai>.Filter.Eq(x => x.isDelete, false);
            var khuyenMai = await _collection.Find(filter).FirstOrDefaultAsync();

            if (khuyenMai == null)
            {
                return new RespondAPI<KhuyenMaiRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy khuyến mãi với ID đã cung cấp."
                );
            }

            _mapper.Map(request, khuyenMai);

            khuyenMai.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, khuyenMai);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<KhuyenMaiRespond>(
                    ResultRespond.Error,
                    "Cập nhật khuyến mãi không thành công."
                );
            }

            var khuyenMaiRespond = _mapper.Map<KhuyenMaiRespond>(khuyenMai);

            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Cập nhật khuyến mãi thành công.",
                khuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<KhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật khuyến mãi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteKhuyenMai(string id)
    {
        try
        {
            var existingKhuyenMai = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingKhuyenMai == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy khuyến mãi để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa khuyến mãi không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa khuyến mãi thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa khuyến mãi: {ex.Message}"
            );
        }
    }
}