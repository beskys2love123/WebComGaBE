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
using repo_nha_hang_com_ga_BE.Models.Responds.NhaCungCap;
using repo_nha_hang_com_ga_BE.Models.Requests;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class NhaCungCapRepository : INhaCungCapRepository
{
    private readonly IMongoCollection<NhaCungCap> _collection;
    private readonly IMapper _mapper;

    public NhaCungCapRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<NhaCungCap>("NhaCungCap");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<NhaCungCapRespond>>> GetAllNhaCungCaps(RequestSearchNhaCungCap request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<NhaCungCap>.Filter.Empty;
            filter &= Builders<NhaCungCap>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenNhaCungCap))
            {
                filter &= Builders<NhaCungCap>.Filter.Regex(x => x.tenNhaCungCap, new BsonRegularExpression($".*{request.tenNhaCungCap}.*"));

            }
            
            if (!string.IsNullOrEmpty(request.diaChi))
            {
                filter &= Builders<NhaCungCap>.Filter.Regex(x => x.diaChi, new BsonRegularExpression($".*{request.diaChi}.*"));

            }


            var projection = Builders<NhaCungCap>.Projection
                .Include(x => x.tenNhaCungCap)
                .Include(x => x.diaChi)
                .Include(x => x.soDienThoai);

            var findOptions = new FindOptions<NhaCungCap, NhaCungCapRespond>
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
                var NhaCungCaps = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecord);
                var pagingResponse = new PagingResponse<List<NhaCungCapRespond>>
                {
                    Paging = pagingDetail,
                    Data = NhaCungCaps
                };

                return new RespondAPIPaging<List<NhaCungCapRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var NhaCungCaps = await cursor.ToListAsync();

                return new RespondAPIPaging<List<NhaCungCapRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<NhaCungCapRespond>>
                    {
                        Data = NhaCungCaps,
                        Paging = new PagingDetail(1, NhaCungCaps.Count, NhaCungCaps.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<NhaCungCapRespond>>(
                ResultRespond.Failed,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<NhaCungCapRespond>> GetNhaCungCapById(string id)
    {
        try
        {
            var NhaCungCap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (NhaCungCap == null)
            {
                return new RespondAPI<NhaCungCapRespond>(
                    ResultRespond.Failed,
                    message: "Nhà cung cấp không tồn tại"
                );
            }

            var NhaCungCapRespond = _mapper.Map<NhaCungCapRespond>(NhaCungCap);

            return new RespondAPI<NhaCungCapRespond>(
                ResultRespond.Succeeded,
                "Lấy thông tin nhà cung cấp thành công",
                NhaCungCapRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<NhaCungCapRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<NhaCungCapRespond>> CreateNhaCungCap(RequestAddNhaCungCap request)
    {
        try
        {
            NhaCungCap newNhaCungCap = _mapper.Map<NhaCungCap>(request);

            newNhaCungCap.isDelete = false;
            newNhaCungCap.createdDate = DateTime.Now;
            newNhaCungCap.updatedDate = DateTime.Now;

            await _collection.InsertOneAsync(newNhaCungCap);

            var NhaCungCapRespond = _mapper.Map<NhaCungCapRespond>(newNhaCungCap);

            return new RespondAPI<NhaCungCapRespond>(
                ResultRespond.Succeeded,
                "Thêm nhà cung cấp thành công",
                NhaCungCapRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<NhaCungCapRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<NhaCungCapRespond>> UpdateNhaCungCap(string id, RequestUpdateNhaCungCap request)
    {
        try
        {
            var filter = Builders<NhaCungCap>.Filter.Eq(x => x.Id, id);
            filter &= Builders<NhaCungCap>.Filter.Eq(x => x.isDelete, false);
            var NhaCungCap = await _collection.Find(filter).FirstOrDefaultAsync();

            if (NhaCungCap == null)
            {
                return new RespondAPI<NhaCungCapRespond>(
                    ResultRespond.NotFound,
                    "Nhà cung cấp không tồn tại."
                );
            }

            _mapper.Map(request, NhaCungCap);

            NhaCungCap.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, NhaCungCap);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<NhaCungCapRespond>(
                    ResultRespond.Error,
                    "Cập nhật nhà cung cấp không thành công."
                );
            }
            var NhaCungCapRespond = _mapper.Map<NhaCungCapRespond>(NhaCungCap);

            return new RespondAPI<NhaCungCapRespond>(
                ResultRespond.Succeeded,
                "Cập nhật nhà cung cấp thành công",
                NhaCungCapRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<NhaCungCapRespond>
            (
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật thông tin nhà cung cấp: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteNhaCungCap(string id)
    {
        try
        {
            var existingNhaCungCap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (existingNhaCungCap == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    message: "Nhà cung cấp không tồn tại"
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa nhà cung cấp không thành công."
                );
            }
            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa nhà cung cấp thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa nhà cung cấp: {ex.Message}"
            );
        }
    }

}