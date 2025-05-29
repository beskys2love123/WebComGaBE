using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.CaLamViec;
using repo_nha_hang_com_ga_BE.Models.Responds.CaLamViecRespond;

namespace repo_nha_hang_com_ga_BE.Models.Repositories.Imp;

public class CaLamViecRepository : ICaLamViecRepository
{
    private readonly IMongoCollection<CaLamViec> _collection;
    private readonly IMapper _mapper;

    public CaLamViecRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<CaLamViec>("CaLamViec");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<CaLamViecRespond>>> GetAllCaLamViec(RequestSearchCaLamViec request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<CaLamViec>.Filter.Empty;
            filter &= Builders<CaLamViec>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenCaLamViec))
            {
                filter &= Builders<CaLamViec>.Filter.Regex(x => x.tenCaLamViec, new BsonRegularExpression($".*{request.tenCaLamViec}.*"));
            }
            if (!string.IsNullOrEmpty(request.khungThoiGian))
            {
                filter &= Builders<CaLamViec>.Filter.Eq(x => x.khungThoiGian, request.khungThoiGian);
            }

            var projection = Builders<CaLamViec>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenCaLamViec)
                .Include(x => x.khungThoiGian)
                .Include(x => x.moTa);


            var findOptions = new FindOptions<CaLamViec, CaLamViecRespond>
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
                var CaLamViecs = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<CaLamViecRespond>>
                {
                    Paging = pagingDetail,
                    Data = CaLamViecs
                };

                return new RespondAPIPaging<List<CaLamViecRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var CaLamViecs = await cursor.ToListAsync();

                return new RespondAPIPaging<List<CaLamViecRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<CaLamViecRespond>>
                    {
                        Data = CaLamViecs,
                        Paging = new PagingDetail(1, CaLamViecs.Count, CaLamViecs.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<CaLamViecRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<CaLamViecRespond>> GetCaLamViecById(string id)
    {
        try
        {
            var CaLamViec = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (CaLamViec == null)
            {
                return new RespondAPI<CaLamViecRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy ca làm việc với ID đã cung cấp."
                );
            }

            var CaLamViecRespond = _mapper.Map<CaLamViecRespond>(CaLamViec);

            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Succeeded,
                "Lấy ca làm việc thành công.",
                CaLamViecRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<CaLamViecRespond>> CreateCaLamViec(RequestAddCaLamViec request)
    {
        try
        {
            CaLamViec newCaLamViec = _mapper.Map<CaLamViec>(request);

            newCaLamViec.createdDate = DateTimeOffset.UtcNow;
            newCaLamViec.updatedDate = DateTimeOffset.UtcNow;
            newCaLamViec.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newDanhMucMonAn.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newCaLamViec);

            var CaLamViecRespond = _mapper.Map<CaLamViecRespond>(newCaLamViec);

            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Succeeded,
                "Tạo ca làm việc thành công.",
                CaLamViecRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo ca làm việc: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<CaLamViecRespond>> UpdateCaLamViec(string id, RequestUpdateCaLamViec request)
    {
        try
        {
            var filter = Builders<CaLamViec>.Filter.Eq(x => x.Id, id);
            filter &= Builders<CaLamViec>.Filter.Eq(x => x.isDelete, false);
            var CaLamViec = await _collection.Find(filter).FirstOrDefaultAsync();

            if (CaLamViec == null)
            {
                return new RespondAPI<CaLamViecRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy ca làm việc với ID đã cung cấp."
                );
            }

            _mapper.Map(request, CaLamViec);

            CaLamViec.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, CaLamViec);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<CaLamViecRespond>(
                    ResultRespond.Error,
                    "Cập nhật ca làm việc không thành công."
                );
            }

            var CaLamViecRespond = _mapper.Map<CaLamViecRespond>(CaLamViec);

            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Succeeded,
                "Cập nhật ca làm việc thành công.",
                CaLamViecRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<CaLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật ca làm việc: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteCaLamViec(string id)
    {
        try
        {
            var existingCaLamViec = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingCaLamViec == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy ca làm việc để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa ca làm việc không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa ca làm việc thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa ca làm việc: {ex.Message}"
            );
        }
    }


}