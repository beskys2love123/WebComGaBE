using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiBan;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiBan;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class LoaiBanRepository : ILoaiBanRepository
{
    private readonly IMongoCollection<LoaiBan> _collection;
    private readonly IMapper _mapper;

    public LoaiBanRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<LoaiBan>("LoaiBan");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<LoaiBanRespond>>> GetAllLoaiBans(RequestSearchLoaiBan request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<LoaiBan>.Filter.Empty;
            filter &= Builders<LoaiBan>.Filter.Eq(x => x.isDelete, false);


            if (!string.IsNullOrEmpty(request.tenLoai))
            {
                filter &= Builders<LoaiBan>.Filter.Regex(x => x.tenLoai, new BsonRegularExpression($".*{request.tenLoai}.*"));
            }

            var projection = Builders<LoaiBan>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<LoaiBan, LoaiBanRespond>
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
                var loaiBans = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<LoaiBanRespond>>
                {
                    Paging = pagingDetail,
                    Data = loaiBans
                };

                return new RespondAPIPaging<List<LoaiBanRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var loaiBans = await cursor.ToListAsync();

                return new RespondAPIPaging<List<LoaiBanRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<LoaiBanRespond>>
                    {
                        Data = loaiBans,
                        Paging = new PagingDetail(1, loaiBans.Count, loaiBans.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<LoaiBanRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<LoaiBanRespond>> GetLoaiBanById(string id)
    {
        try
        {
            var loaiBan = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (loaiBan == null)
            {
                return new RespondAPI<LoaiBanRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại bàn với ID đã cung cấp."
                );
            }

            var loaiBanRespond = _mapper.Map<LoaiBanRespond>(loaiBan);

            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Succeeded,
                "Lấy loại bàn thành công.",
                loaiBanRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiBanRespond>> CreateLoaiBan(RequestAddLoaiBan request)
    {
        try
        {
            LoaiBan newLoaiBan = _mapper.Map<LoaiBan>(request);

            newLoaiBan.createdDate = DateTimeOffset.UtcNow;
            newLoaiBan.updatedDate = DateTimeOffset.UtcNow;
            newLoaiBan.isDelete = false;

            await _collection.InsertOneAsync(newLoaiBan);

            var loaiBanRespond = _mapper.Map<LoaiBanRespond>(newLoaiBan);

            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Succeeded,
                "Tạo loại bàn thành công.",
                loaiBanRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo loại bàn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LoaiBanRespond>> UpdateLoaiBan(string id, RequestUpdateLoaiBan request)
    {
        try
        {
            var filter = Builders<LoaiBan>.Filter.Eq(x => x.Id, id);
            filter &= Builders<LoaiBan>.Filter.Eq(x => x.isDelete, false);
            var loaiBan = await _collection.Find(filter).FirstOrDefaultAsync();

            if (loaiBan == null)
            {
                return new RespondAPI<LoaiBanRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại bàn với ID đã cung cấp."
                );
            }

            _mapper.Map(request, loaiBan);

            loaiBan.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, loaiBan);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<LoaiBanRespond>(
                    ResultRespond.Error,
                    "Cập nhật loại bàn không thành công."
                );
            }

            var loaiBanRespond = _mapper.Map<LoaiBanRespond>(loaiBan);

            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Succeeded,
                "Cập nhật loại bàn thành công.",
                loaiBanRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LoaiBanRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật loại bàn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteLoaiBan(string id)
    {
        try
        {
            var existingLoaiBan = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingLoaiBan == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy loại bàn để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa loại bàn không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa loại bàn thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa loại bàn: {ex.Message}"
            );
        }
    }
}