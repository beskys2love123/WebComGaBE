using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucMonAn;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class DanhMucMonAnRepository : IDanhMucMonAnRepository
{
    private readonly IMongoCollection<DanhMucMonAn> _collection;
    private readonly IMapper _mapper;

    public DanhMucMonAnRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<DanhMucMonAn>("DanhMucMonAn");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<DanhMucMonAnRespond>>> GetAllDanhMucMonAns(RequestSearchDanhMucMonAn request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<DanhMucMonAn>.Filter.Empty;
            filter &= Builders<DanhMucMonAn>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenDanhMuc))
            {
                filter &= Builders<DanhMucMonAn>.Filter.Regex(x => x.tenDanhMuc, new BsonRegularExpression($".*{request.tenDanhMuc}.*"));
            }

            var projection = Builders<DanhMucMonAn>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDanhMuc)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<DanhMucMonAn, DanhMucMonAnRespond>
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
                var danhMucMonAns = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<DanhMucMonAnRespond>>
                {
                    Paging = pagingDetail,
                    Data = danhMucMonAns
                };

                return new RespondAPIPaging<List<DanhMucMonAnRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var danhMucMonAns = await cursor.ToListAsync();

                return new RespondAPIPaging<List<DanhMucMonAnRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<DanhMucMonAnRespond>>
                    {
                        Data = danhMucMonAns,
                        Paging = new PagingDetail(1, danhMucMonAns.Count, danhMucMonAns.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<DanhMucMonAnRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<DanhMucMonAnRespond>> GetDanhMucMonAnById(string id)
    {
        try
        {
            var danhMucMonAn = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (danhMucMonAn == null)
            {
                return new RespondAPI<DanhMucMonAnRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục món ăn với ID đã cung cấp."
                );
            }

            var danhMucMonAnRespond = _mapper.Map<DanhMucMonAnRespond>(danhMucMonAn);

            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Succeeded,
                "Lấy danh mục món ăn thành công.",
                danhMucMonAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DanhMucMonAnRespond>> CreateDanhMucMonAn(RequestAddDanhMucMonAn request)
    {
        try
        {
            DanhMucMonAn newDanhMucMonAn = _mapper.Map<DanhMucMonAn>(request);

            newDanhMucMonAn.createdDate = DateTimeOffset.UtcNow;
            newDanhMucMonAn.updatedDate = DateTimeOffset.UtcNow;
            newDanhMucMonAn.isDelete = false;

            await _collection.InsertOneAsync(newDanhMucMonAn);

            var danhMucMonAnRespond = _mapper.Map<DanhMucMonAnRespond>(newDanhMucMonAn);

            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Succeeded,
                "Tạo danh mục món ăn thành công.",
                danhMucMonAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo danh mục món ăn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DanhMucMonAnRespond>> UpdateDanhMucMonAn(string id, RequestUpdateDanhMucMonAn request)
    {
        try
        {
            var filter = Builders<DanhMucMonAn>.Filter.Eq(x => x.Id, id);
            filter &= Builders<DanhMucMonAn>.Filter.Eq(x => x.isDelete, false);
            var danhMucMonAn = await _collection.Find(filter).FirstOrDefaultAsync();

            if (danhMucMonAn == null)
            {
                return new RespondAPI<DanhMucMonAnRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục món ăn với ID đã cung cấp."
                );
            }

            _mapper.Map(request, danhMucMonAn);

            danhMucMonAn.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, danhMucMonAn);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<DanhMucMonAnRespond>(
                    ResultRespond.Error,
                    "Cập nhật danh mục món ăn không thành công."
                );
            }

            var danhMucMonAnRespond = _mapper.Map<DanhMucMonAnRespond>(danhMucMonAn);

            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Succeeded,
                "Cập nhật danh mục món ăn thành công.",
                danhMucMonAnRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucMonAnRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật danh mục món ăn: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteDanhMucMonAn(string id)
    {
        try
        {
            var existingDanhMuc = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingDanhMuc == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục món ăn để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa danh mục món ăn không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa danh mục món ăn thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa danh mục món ăn: {ex.Message}"
            );
        }
    }
}