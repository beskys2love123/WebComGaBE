using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiKhuyenMai;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class LoaiKhuyenMaiRepository : ILoaiKhuyenMaiRepository
{
    private readonly IMongoCollection<LoaiKhuyenMai> _collection;
    private readonly IMapper _mapper;

    public LoaiKhuyenMaiRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<LoaiKhuyenMai>("LoaiKhuyenMai");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<LoaiKhuyenMaiRespond>>> GetAllLoaiKhuyenMais(RequestSearchLoaiKhuyenMai request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<LoaiKhuyenMai>.Filter.Empty;
            filter &= Builders<LoaiKhuyenMai>.Filter.Eq(x => x.isDelete, false);


            if (!string.IsNullOrEmpty(request.tenLoai))
            {
                filter &= Builders<LoaiKhuyenMai>.Filter.Regex(x => x.tenLoai, new BsonRegularExpression($".*{request.tenLoai}.*"));
            }

            var projection = Builders<LoaiKhuyenMai>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<LoaiKhuyenMai, LoaiKhuyenMaiRespond>
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
                var loaiKhuyenMais = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<LoaiKhuyenMaiRespond>>
                {
                    Paging = pagingDetail,
                    Data = loaiKhuyenMais
                };

                return new RespondAPIPaging<List<LoaiKhuyenMaiRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var loaiKhuyenMais = await cursor.ToListAsync();

                return new RespondAPIPaging<List<LoaiKhuyenMaiRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<LoaiKhuyenMaiRespond>>
                    {
                        Data = loaiKhuyenMais,
                        Paging = new PagingDetail(1, loaiKhuyenMais.Count, loaiKhuyenMais.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<LoaiKhuyenMaiRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<LoaiKhuyenMaiRespond>> GetLoaiKhuyenMaiById(string id)
    {
        try
        {
            var loaiKhuyenMai = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (loaiKhuyenMai == null)
            {
                return new RespondAPI<LoaiKhuyenMaiRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại khuyến mãi với ID đã cung cấp."
                );
            }

            var loaiKhuyenMaiRespond = _mapper.Map<LoaiKhuyenMaiRespond>(loaiKhuyenMai);

            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Lấy loại khuyến mãi thành công.",
                loaiKhuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiKhuyenMaiRespond>> CreateLoaiKhuyenMai(RequestAddLoaiKhuyenMai request)
    {
        try
        {
            LoaiKhuyenMai newLoaiKhuyenMai = _mapper.Map<LoaiKhuyenMai>(request);

            newLoaiKhuyenMai.createdDate = DateTimeOffset.UtcNow;
            newLoaiKhuyenMai.updatedDate = DateTimeOffset.UtcNow;
            newLoaiKhuyenMai.isDelete = false;

            await _collection.InsertOneAsync(newLoaiKhuyenMai);

            var loaiKhuyenMaiRespond = _mapper.Map<LoaiKhuyenMaiRespond>(newLoaiKhuyenMai);

            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Tạo loại khuyến mãi thành công.",
                loaiKhuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo loại khuyến mãi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiKhuyenMaiRespond>> UpdateLoaiKhuyenMai(string id, RequestUpdateLoaiKhuyenMai request)
    {
        try
        {
            var filter = Builders<LoaiKhuyenMai>.Filter.Eq(x => x.Id, id);
            filter &= Builders<LoaiKhuyenMai>.Filter.Eq(x => x.isDelete, false);
            var loaiKhuyenMai = await _collection.Find(filter).FirstOrDefaultAsync();

            if (loaiKhuyenMai == null)
            {
                return new RespondAPI<LoaiKhuyenMaiRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại khuyến mãi với ID đã cung cấp."
                );
            }

            _mapper.Map(request, loaiKhuyenMai);

            loaiKhuyenMai.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, loaiKhuyenMai);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<LoaiKhuyenMaiRespond>(
                    ResultRespond.Error,
                    "Cập nhật loại khuyến mãi không thành công."
                );
            }

            var loaiKhuyenMaiRespond = _mapper.Map<LoaiKhuyenMaiRespond>(loaiKhuyenMai);

            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Succeeded,
                "Cập nhật loại khuyến mãi thành công.",
                loaiKhuyenMaiRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiKhuyenMaiRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật loại khuyến mãi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteLoaiKhuyenMai(string id)
    {
        try
        {
            var existingLoaiKhuyenMai = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingLoaiKhuyenMai == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại khuyến mãi để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa loại khuyến mãi không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa loại khuyến mãi thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa loại khuyến mãi: {ex.Message}"
            );
        }
    }
}