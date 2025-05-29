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
using repo_nha_hang_com_ga_BE.Models.Responds.PhanQuyen;
using repo_nha_hang_com_ga_BE.Models.Requests;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhanQuyenRepository : IPhanQuyenRepository
{
    private readonly IMongoCollection<PhanQuyen> _collection;
    private readonly IMapper _mapper;

    public PhanQuyenRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhanQuyen>("PhanQuyen");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<PhanQuyenRespond>>> GetAllPhanQuyens(RequestSearchPhanQuyen request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<PhanQuyen>.Filter.Empty;
            filter &= Builders<PhanQuyen>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhanQuyen))
            {
                filter &= Builders<PhanQuyen>.Filter.Regex(x => x.tenPhanQuyen, new BsonRegularExpression($".*{request.tenPhanQuyen}.*"));

            }


            var projection = Builders<PhanQuyen>.Projection
                .Include(x => x.tenPhanQuyen)
                .Include(x => x.moTa)
                .Include(x => x.danhSachMenu);

            var findOptions = new FindOptions<PhanQuyen, PhanQuyenRespond>
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
                var PhanQuyens = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecord);
                var pagingResponse = new PagingResponse<List<PhanQuyenRespond>>
                {
                    Paging = pagingDetail,
                    Data = PhanQuyens
                };

                return new RespondAPIPaging<List<PhanQuyenRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var PhanQuyens = await cursor.ToListAsync();

                return new RespondAPIPaging<List<PhanQuyenRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhanQuyenRespond>>
                    {
                        Data = PhanQuyens,
                        Paging = new PagingDetail(1, PhanQuyens.Count, PhanQuyens.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhanQuyenRespond>>(
                ResultRespond.Failed,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhanQuyenRespond>> GetPhanQuyenById(string id)
    {
        try
        {
            var PhanQuyen = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (PhanQuyen == null)
            {
                return new RespondAPI<PhanQuyenRespond>(
                    ResultRespond.Failed,
                    message: "Phân quyền không tồn tại"
                );
            }

            var PhanQuyenRespond = _mapper.Map<PhanQuyenRespond>(PhanQuyen);

            return new RespondAPI<PhanQuyenRespond>(
                ResultRespond.Succeeded,
                "Lấy thông tin Phân quyền thành công",
                PhanQuyenRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhanQuyenRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<PhanQuyenRespond>> CreatePhanQuyen(RequestAddPhanQuyen request)
    {
        try
        {
            PhanQuyen newPhanQuyen = _mapper.Map<PhanQuyen>(request);

            newPhanQuyen.isDelete = false;
            newPhanQuyen.createdDate = DateTime.Now;
            newPhanQuyen.updatedDate = DateTime.Now;

            await _collection.InsertOneAsync(newPhanQuyen);

            var PhanQuyenRespond = _mapper.Map<PhanQuyenRespond>(newPhanQuyen);

            return new RespondAPI<PhanQuyenRespond>(
                ResultRespond.Succeeded,
                "Thêm Phân quyền thành công",
                PhanQuyenRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhanQuyenRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<PhanQuyenRespond>> UpdatePhanQuyen(string id, RequestUpdatePhanQuyen request)
    {
        try
        {
            var filter = Builders<PhanQuyen>.Filter.Eq(x => x.Id, id);
            filter &= Builders<PhanQuyen>.Filter.Eq(x => x.isDelete, false);
            var PhanQuyen = await _collection.Find(filter).FirstOrDefaultAsync();

            if (PhanQuyen == null)
            {
                return new RespondAPI<PhanQuyenRespond>(
                    ResultRespond.NotFound,
                    "Phân quyền không tồn tại."
                );
            }

            _mapper.Map(request, PhanQuyen);

            PhanQuyen.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, PhanQuyen);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<PhanQuyenRespond>(
                    ResultRespond.Error,
                    "Cập nhật Phân quyền không thành công."
                );
            }
            var PhanQuyenRespond = _mapper.Map<PhanQuyenRespond>(PhanQuyen);

            return new RespondAPI<PhanQuyenRespond>(
                ResultRespond.Succeeded,
                "Cập nhật Phân quyền thành công",
                PhanQuyenRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhanQuyenRespond>
            (
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật thông tin Phân quyền: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeletePhanQuyen(string id)
    {
        try
        {
            var existingPhanQuyen = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (existingPhanQuyen == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    message: "Phân quyền không tồn tại"
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa Phân quyền không thành công."
                );
            }
            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa Phân quyền thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa Phân quyền: {ex.Message}"
            );
        }
    }
}