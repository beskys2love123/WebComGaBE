using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class DanhMucNguyenLieuRepository : IDanhMucNguyenLieuRepository
{
    private readonly IMongoCollection<DanhMucNguyenLieu> _collection;
    private readonly IMapper _mapper;

    public DanhMucNguyenLieuRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<DanhMucNguyenLieu>("DanhMucNguyenLieu");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<DanhMucNguyenLieuRespond>>> GetAllDanhMucNguyenLieus(RequestSearchDanhMucNguyenLieu request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<DanhMucNguyenLieu>.Filter.Empty;
            filter &= Builders<DanhMucNguyenLieu>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenDanhMuc))
            {
                filter &= Builders<DanhMucNguyenLieu>.Filter.Regex(x => x.tenDanhMuc, new BsonRegularExpression($".*{request.tenDanhMuc}.*"));
            }

            var projection = Builders<DanhMucNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDanhMuc)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<DanhMucNguyenLieu, DanhMucNguyenLieuRespond>
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
                var danhMucNguyenLieus = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<DanhMucNguyenLieuRespond>>
                {
                    Paging = pagingDetail,
                    Data = danhMucNguyenLieus
                };

                return new RespondAPIPaging<List<DanhMucNguyenLieuRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var danhMucNguyenLieus = await cursor.ToListAsync();

                return new RespondAPIPaging<List<DanhMucNguyenLieuRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<DanhMucNguyenLieuRespond>>
                    {
                        Data = danhMucNguyenLieus,
                        Paging = new PagingDetail(1, danhMucNguyenLieus.Count, danhMucNguyenLieus.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<DanhMucNguyenLieuRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<DanhMucNguyenLieuRespond>> GetDanhMucNguyenLieuById(string id)
    {
        try
        {
            var danhMucNguyenLieu = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (danhMucNguyenLieu == null)
            {
                return new RespondAPI<DanhMucNguyenLieuRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục nguyên liệu với ID đã cung cấp."
                );
            }

            var danhMucNguyenLieuRespond = _mapper.Map<DanhMucNguyenLieuRespond>(danhMucNguyenLieu);

            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Lấy danh mục nguyên liệu thành công.",
                danhMucNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DanhMucNguyenLieuRespond>> CreateDanhMucNguyenLieu(RequestAddDanhMucNguyenLieu request)
    {
        try
        {
            DanhMucNguyenLieu newDanhMucNguyenLieu = _mapper.Map<DanhMucNguyenLieu>(request);

            newDanhMucNguyenLieu.createdDate = DateTimeOffset.UtcNow;
            newDanhMucNguyenLieu.updatedDate = DateTimeOffset.UtcNow;
            newDanhMucNguyenLieu.isDelete = false;



            await _collection.InsertOneAsync(newDanhMucNguyenLieu);

            var danhMucNguyenLieuRespond = _mapper.Map<DanhMucNguyenLieuRespond>(newDanhMucNguyenLieu);

            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Tạo danh mục nguyên liệu thành công.",
                danhMucNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo danh mục nguyên liệu: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DanhMucNguyenLieuRespond>> UpdateDanhMucNguyenLieu(string id, RequestUpdateDanhMucNguyenLieu request)
    {
        try
        {
            var filter = Builders<DanhMucNguyenLieu>.Filter.Eq(x => x.Id, id);
            filter &= Builders<DanhMucNguyenLieu>.Filter.Eq(x => x.isDelete, false);
            var danhMucNguyenLieu = await _collection.Find(filter).FirstOrDefaultAsync();

            if (danhMucNguyenLieu == null)
            {
                return new RespondAPI<DanhMucNguyenLieuRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục nguyên liệu với ID đã cung cấp."
                );
            }

            _mapper.Map(request, danhMucNguyenLieu);

            danhMucNguyenLieu.updatedDate = DateTimeOffset.UtcNow;


            var updateResult = await _collection.ReplaceOneAsync(filter, danhMucNguyenLieu);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<DanhMucNguyenLieuRespond>(
                    ResultRespond.Error,
                    "Cập nhật danh mục nguyên liệu không thành công."
                );
            }

            var danhMucNguyenLieuRespond = _mapper.Map<DanhMucNguyenLieuRespond>(danhMucNguyenLieu);

            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Succeeded,
                "Cập nhật danh mục nguyên liệu thành công.",
                danhMucNguyenLieuRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DanhMucNguyenLieuRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật danh mục nguyên liệu: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteDanhMucNguyenLieu(string id)
    {
        try
        {
            var existingDanhMuc = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingDanhMuc == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy danh mục nguyên liệu để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa danh mục nguyên liệu không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa danh mục nguyên liệu thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa danh mục nguyên liệu: {ex.Message}"
            );
        }
    }
}