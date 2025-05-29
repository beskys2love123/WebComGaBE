using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiTuDo;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiTuDo;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class LoaiTuDoRepository : ILoaiTuDoRepository
{
    private readonly IMongoCollection<LoaiTuDo> _collection;
    private readonly IMapper _mapper;

    public LoaiTuDoRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<LoaiTuDo>("LoaiTuDo");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<LoaiTuDoRespond>>> GetAllLoaiTuDos(RequestSearchLoaiTuDo request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<LoaiTuDo>.Filter.Empty;
            filter &= Builders<LoaiTuDo>.Filter.Eq(x => x.isDelete, false);


            if (!string.IsNullOrEmpty(request.tenLoai))
            {
                filter &= Builders<LoaiTuDo>.Filter.Regex(x => x.tenLoai, new BsonRegularExpression($".*{request.tenLoai}.*"));
            }

            var projection = Builders<LoaiTuDo>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<LoaiTuDo, LoaiTuDoRespond>
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
                var loaiTuDos = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<LoaiTuDoRespond>>
                {
                    Paging = pagingDetail,
                    Data = loaiTuDos
                };

                return new RespondAPIPaging<List<LoaiTuDoRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var loaiTuDos = await cursor.ToListAsync();

                return new RespondAPIPaging<List<LoaiTuDoRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<LoaiTuDoRespond>>
                    {
                        Data = loaiTuDos,
                        Paging = new PagingDetail(1, loaiTuDos.Count, loaiTuDos.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<LoaiTuDoRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<LoaiTuDoRespond>> GetLoaiTuDoById(string id)
    {
        try
        {
            var loaiTuDo = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (loaiTuDo == null)
            {
                return new RespondAPI<LoaiTuDoRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại tủ đồ với ID đã cung cấp."
                );
            }

            var loaiTuDoRespond = _mapper.Map<LoaiTuDoRespond>(loaiTuDo);

            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Succeeded,
                "Lấy loại tủ đồ thành công.",
                loaiTuDoRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiTuDoRespond>> CreateLoaiTuDo(RequestAddLoaiTuDo request)
    {
        try
        {
            LoaiTuDo newLoaiTuDo = _mapper.Map<LoaiTuDo>(request);

            newLoaiTuDo.createdDate = DateTimeOffset.UtcNow;
            newLoaiTuDo.updatedDate = DateTimeOffset.UtcNow;
            newLoaiTuDo.isDelete = false;

            await _collection.InsertOneAsync(newLoaiTuDo);

            var loaiTuDoRespond = _mapper.Map<LoaiTuDoRespond>(newLoaiTuDo);

            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Succeeded,
                "Tạo loại tủ đồ thành công.",
                loaiTuDoRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo loại tủ đồ: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiTuDoRespond>> UpdateLoaiTuDo(string id, RequestUpdateLoaiTuDo request)
    {
        try
        {
            var filter = Builders<LoaiTuDo>.Filter.Eq(x => x.Id, id);
            filter &= Builders<LoaiTuDo>.Filter.Eq(x => x.isDelete, false);
            var loaiTuDo = await _collection.Find(filter).FirstOrDefaultAsync();

            if (loaiTuDo == null)
            {
                return new RespondAPI<LoaiTuDoRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại tủ đồ với ID đã cung cấp."
                );
            }

            _mapper.Map(request, loaiTuDo);

            loaiTuDo.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, loaiTuDo);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<LoaiTuDoRespond>(
                    ResultRespond.Error,
                    "Cập nhật loại tủ đồ không thành công."
                );
            }

            var loaiTuDoRespond = _mapper.Map<LoaiTuDoRespond>(loaiTuDo);

            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Succeeded,
                "Cập nhật loại tủ đồ thành công.",
                loaiTuDoRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiTuDoRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật loại tủ đồ: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteLoaiTuDo(string id)
    {
        try
        {
            var existingLoaiTuDo = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingLoaiTuDo == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại tủ đồ để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa loại tủ đồ không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa loại tủ đồ thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa loại tủ đồ: {ex.Message}"
            );
        }
    }
}