using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DonViTinh;
using repo_nha_hang_com_ga_BE.Models.Responds.DonViTinh;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class DonViTinhRepository : IDonViTinhRepository
{
    private readonly IMongoCollection<DonViTinh> _collection;
    private readonly IMapper _mapper;

    public DonViTinhRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<DonViTinh>("DonViTinh");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<DonViTinhRespond>>> GetAllDonViTinhs(RequestSearchDonViTinh request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<DonViTinh>.Filter.Empty;
            filter &= Builders<DonViTinh>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenDonViTinh))
            {
                filter &= Builders<DonViTinh>.Filter.Regex(x => x.tenDonViTinh, new BsonRegularExpression($".*{request.tenDonViTinh}.*"));
            }

            var projection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh)
                .Include(x => x.moTa);

            var findOptions = new FindOptions<DonViTinh, DonViTinhRespond>
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
                var donViTinhs = await cursor.ToListAsync();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<DonViTinhRespond>>
                {
                    Paging = pagingDetail,
                    Data = donViTinhs
                };

                return new RespondAPIPaging<List<DonViTinhRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var donViTinhs = await cursor.ToListAsync();

                return new RespondAPIPaging<List<DonViTinhRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<DonViTinhRespond>>
                    {
                        Data = donViTinhs,
                        Paging = new PagingDetail(1, donViTinhs.Count, donViTinhs.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<DonViTinhRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<DonViTinhRespond>> GetDonViTinhById(string id)
    {
        try
        {
            var donViTinh = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (donViTinh == null)
            {
                return new RespondAPI<DonViTinhRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn vị tính với ID đã cung cấp."
                );
            }

            var donViTinhRespond = _mapper.Map<DonViTinhRespond>(donViTinh);

            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Succeeded,
                "Lấy đơn vị tính thành công.",
                donViTinhRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DonViTinhRespond>> CreateDonViTinh(RequestAddDonViTinh request)
    {
        try
        {
            DonViTinh newDonViTinh = _mapper.Map<DonViTinh>(request);

            newDonViTinh.createdDate = DateTimeOffset.UtcNow;
            newDonViTinh.updatedDate = DateTimeOffset.UtcNow;
            newDonViTinh.isDelete = false;

            await _collection.InsertOneAsync(newDonViTinh);

            var donViTinhRespond = _mapper.Map<DonViTinhRespond>(newDonViTinh);

            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Succeeded,
                "Tạo đơn vị tính thành công.",
                donViTinhRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo đơn vị tính: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DonViTinhRespond>> UpdateDonViTinh(string id, RequestUpdateDonViTinh request)
    {
        try
        {
            var filter = Builders<DonViTinh>.Filter.Eq(x => x.Id, id);
            filter &= Builders<DonViTinh>.Filter.Eq(x => x.isDelete, false);
            var donViTinh = await _collection.Find(filter).FirstOrDefaultAsync();

            if (donViTinh == null)
            {
                return new RespondAPI<DonViTinhRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn vị tính với ID đã cung cấp."
                );
            }

            _mapper.Map(request, donViTinh);

            donViTinh.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, donViTinh);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<DonViTinhRespond>(
                    ResultRespond.Error,
                    "Cập nhật đơn vị tính không thành công."
                );
            }

            var donViTinhRespond = _mapper.Map<DonViTinhRespond>(donViTinh);

            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Succeeded,
                "Cập nhật đơn vị tính thành công.",
                donViTinhRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonViTinhRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật đơn vị tính: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteDonViTinh(string id)
    {
        try
        {
            var existingDonViTinh = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingDonViTinh == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn vị tính để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa đơn vị tính không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa đơn vị tính thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa đơn vị tính: {ex.Message}"
            );
        }
    }
}