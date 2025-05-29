using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuKiemKe;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuKiemKe;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhieuKiemKeRepository : IPhieuKiemKeRepository
{
    private readonly IMongoCollection<PhieuKiemKe> _collection;
    private readonly IMongoCollection<LoaiNguyenLieu> _collectionLoaiNguyenLieu;
    private readonly IMongoCollection<NguyenLieu> _collectionNguyenLieu;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;
    private readonly IMapper _mapper;

    public PhieuKiemKeRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhieuKiemKe>("PhieuKiemKe");
        _collectionLoaiNguyenLieu = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _collectionNguyenLieu = database.GetCollection<NguyenLieu>("NguyenLieu");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");
        _mapper = mapper;
    }
    public async Task<RespondAPIPaging<List<PhieuKiemKeRespond>>> GetAllPhieuKiemKes(RequestSearchPhieuKiemKe request)
    {
        try
        {
            var collection = _collection;
            var filter = Builders<PhieuKiemKe>.Filter.Empty;
            filter &= Builders<PhieuKiemKe>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhieu))
            {
                filter &= Builders<PhieuKiemKe>.Filter.Regex(x => x.tenPhieu, new BsonRegularExpression($".*{request.tenPhieu}.*"));
            }

            if (request.tuNgay != null)
            {
                filter &= Builders<PhieuKiemKe>.Filter.Gte(x => x.ngayKiemKe, request.tuNgay.Value);
            }


            if (request.denNgay != null)
            {
                filter &= Builders<PhieuKiemKe>.Filter.Lte(x => x.ngayKiemKe, request.denNgay.Value);
            }

            var projection = Builders<PhieuKiemKe>.Projection
               .Include(x => x.Id)
               .Include(x => x.tenPhieu)
               .Include(x => x.createdDate)
               .Include(x => x.ngayKiemKe)
               .Include(x => x.diaDiem)
               .Include(x => x.ghiChu)
               .Include(x => x.nhanVien)
               .Include(x => x.loaiNguyenLieus);

            var findOptions = new FindOptions<PhieuKiemKe, PhieuKiemKe>
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
                var phieuKiemKes = await cursor.ToListAsync();

                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuKiemKes.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuKiemKes.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                   .Project<NhanVien>(nhanVienProjection)
                   .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);
                List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
                foreach (var phieuKiemKe in phieuKiemKes)
                {
                    var nguyenLieuIds = phieuKiemKe.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong);
                    var newNguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                        .Project<NguyenLieu>(nguyenLieuProjection)
                        .ToListAsync();

                    var uniqueNguyenLieus = newNguyenLieus.Where(x => !nguyenLieus.Any(y => y.Id == x.Id));
                    nguyenLieus.AddRange(uniqueNguyenLieus);

                    var newDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
                    foreach (var item in newDict)
                    {
                        if (!nguyenLieuDict.ContainsKey(item.Key))
                        {
                            nguyenLieuDict.Add(item.Key, item.Value);
                        }
                    }
                }

                var phieuKiemKeResponds = phieuKiemKes.Select(x => new PhieuKiemKeRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayKiemKe = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuKiemKeRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuKiemKeRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            soLuongThucTe = z.soLuongThucTe,
                            chenhLech = z.chenhLech,
                            ghiChu = z.ghiChu,
                        }).ToList()
                    }).ToList()
                }).ToList();
                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<PhieuKiemKeRespond>>
                {
                    Paging = pagingDetail,
                    Data = phieuKiemKeResponds
                };
                return new RespondAPIPaging<List<PhieuKiemKeRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var phieuKiemKes = await cursor.ToListAsync();

                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuKiemKes.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuKiemKes.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                   .Project<NhanVien>(nhanVienProjection)
                   .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);
                List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
                foreach (var phieuKiemKe in phieuKiemKes)
                {
                    var nguyenLieuIds = phieuKiemKe.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong);
                    var newNguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                        .Project<NguyenLieu>(nguyenLieuProjection)
                        .ToListAsync();

                    var uniqueNguyenLieus = newNguyenLieus.Where(x => !nguyenLieus.Any(y => y.Id == x.Id));
                    nguyenLieus.AddRange(uniqueNguyenLieus);

                    var newDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
                    foreach (var item in newDict)
                    {
                        if (!nguyenLieuDict.ContainsKey(item.Key))
                        {
                            nguyenLieuDict.Add(item.Key, item.Value);
                        }
                    }
                }

                var phieuKiemKeResponds = phieuKiemKes.Select(x => new PhieuKiemKeRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayKiemKe = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuKiemKeRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuKiemKeRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            soLuongThucTe = z.soLuongThucTe,
                            chenhLech = z.chenhLech,
                            ghiChu = z.ghiChu,
                        }).ToList()
                    }).ToList()
                }).ToList();
                return new RespondAPIPaging<List<PhieuKiemKeRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhieuKiemKeRespond>>
                    {
                        Data = phieuKiemKeResponds,
                        Paging = new PagingDetail(1, phieuKiemKeResponds.Count, phieuKiemKeResponds.Count)
                    }
                );
            }

        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhieuKiemKeRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }

    }
    public async Task<RespondAPI<PhieuKiemKeRespond>> GetPhieuKiemKeById(string id)
    {
        try
        {

            var phieuKiemKe = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (phieuKiemKe == null)
            {
                return new RespondAPI<PhieuKiemKeRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phieu kiểm kê với ID đã cung cấp."
                );
            }
            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = phieuKiemKe.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();

            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);
            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
            var nguyenLieuIds = phieuKiemKe.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
            var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNguyenLieu)
                .Include(x => x.soLuong);
            nguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                .Project<NguyenLieu>(nguyenLieuProjection)
                .ToListAsync();

            nguyenLieuDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);


            var phieuKieResponds = new PhieuKiemKeRespond
            {
                id = phieuKiemKe.Id,
                tenPhieu = phieuKiemKe.tenPhieu,
                ngayKiemKe = phieuKiemKe.createdDate,
                diaDiem = phieuKiemKe.diaDiem,
                ghiChu = phieuKiemKe.ghiChu,
                nhanVien = phieuKiemKe.nhanVien != null ? new IdName
                {
                    Id = phieuKiemKe.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == phieuKiemKe.nhanVien).FirstOrDefault().tenNhanVien
                } : new IdName { Id = "", Name = "" },
                loaiNguyenLieus = phieuKiemKe.loaiNguyenLieus.Select(y => new loaiNguyenLieuKiemKeRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuKiemKeRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(y.id) ? nguyenLieus.FirstOrDefault(m => m.Id == y.id)?.soLuong : null,
                        soLuongThucTe = z.soLuongThucTe,
                        chenhLech = z.chenhLech,
                        ghiChu = z.ghiChu,
                    }).ToList()
                }).ToList()

            };
            return new RespondAPI<PhieuKiemKeRespond>(
                ResultRespond.Succeeded,
                "Lấy phiếu kiểm kê thành công.",
                phieuKieResponds
            );


        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuKiemKeRespond>(
                ResultRespond.Error,
                message: ex.Message
            );
        }

    }

    public async Task<RespondAPI<PhieuKiemKeRespond>> CreatePhieuKiemKe(RequestAddPhieuKiemKe request)
    {
        try
        {
            PhieuKiemKe newPhieuKiemKe = _mapper.Map<PhieuKiemKe>(request);

            newPhieuKiemKe.createdDate = DateTimeOffset.UtcNow;
            newPhieuKiemKe.updatedDate = DateTimeOffset.UtcNow;
            newPhieuKiemKe.isDelete = false;
            newPhieuKiemKe.ngayKiemKe = newPhieuKiemKe.createdDate;

            await _collection.InsertOneAsync(newPhieuKiemKe);

            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = newPhieuKiemKe.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();

            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();

            var nguyenLieuIds = newPhieuKiemKe.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
            var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNguyenLieu)
                .Include(x => x.soLuong);

            nguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                .Project<NguyenLieu>(nguyenLieuProjection)
                .ToListAsync();

            nguyenLieuDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);

            var phieuKiemKeRespond = new PhieuKiemKeRespond
            {
                id = newPhieuKiemKe.Id,
                tenPhieu = newPhieuKiemKe.tenPhieu,
                ngayKiemKe = newPhieuKiemKe.createdDate,
                diaDiem = newPhieuKiemKe.diaDiem,
                ghiChu = newPhieuKiemKe.ghiChu,
                nhanVien = newPhieuKiemKe.nhanVien != null ? new IdName
                {
                    Id = newPhieuKiemKe.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == newPhieuKiemKe.nhanVien).FirstOrDefault().tenNhanVien
                } : null,
                loaiNguyenLieus = newPhieuKiemKe.loaiNguyenLieus.Select(y => new loaiNguyenLieuKiemKeRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuKiemKeRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(y.id) ? nguyenLieus.FirstOrDefault(m => m.Id == y.id)?.soLuong : null,
                        soLuongThucTe = z.soLuongThucTe,
                        chenhLech = z.chenhLech,
                        ghiChu = z.ghiChu,
                    }).ToList()
                }).ToList()
            };

            return new RespondAPI<PhieuKiemKeRespond>(
                ResultRespond.Succeeded,
                "Tạo phiếu kiểm kê thành công.",
                phieuKiemKeRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuKiemKeRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo phiếu kiểm kê: {ex.Message}"
            );
        }

    }

    public async Task<RespondAPI<string>> DeletePhieuKiemKe(string id)
    {
        try
        {
            var existingPhieuKiemKe = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingPhieuKiemKe == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu kiểm kê để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa phiếu kiểm kê không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa phiếu kiểm kê thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa kiểm kê: {ex.Message}"
            );
        }
    }


}