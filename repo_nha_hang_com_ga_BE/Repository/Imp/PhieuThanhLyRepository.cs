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
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuThanhLy;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuThanhLy;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhieuThanhLyRepository : IPhieuThanhLyRepository
{
    private readonly IMongoCollection<PhieuThanhLy> _collection;
    private readonly IMongoCollection<LoaiNguyenLieu> _collectionLoaiNguyenLieu;
    private readonly IMongoCollection<NguyenLieu> _collectionNguyenLieu;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;

    private readonly IMongoCollection<DonViTinh> _collectionDonViTinh;
    private readonly IMapper _mapper;

    public PhieuThanhLyRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhieuThanhLy>("PhieuThanhLy");
        _collectionLoaiNguyenLieu = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _collectionNguyenLieu = database.GetCollection<NguyenLieu>("NguyenLieu");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");
        _collectionDonViTinh = database.GetCollection<DonViTinh>("DonViTinh");
        _mapper = mapper;
    }
    public async Task<RespondAPIPaging<List<PhieuThanhLyRespond>>> GetAllPhieuThanhLys(RequestSearchPhieuThanhLy request)
    {
        try
        {
            var collection = _collection;
            var filter = Builders<PhieuThanhLy>.Filter.Empty;
            filter &= Builders<PhieuThanhLy>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhieu))
            {
                filter &= Builders<PhieuThanhLy>.Filter.Regex(x => x.tenPhieu, new BsonRegularExpression($".*{request.tenPhieu}.*"));
            }

            if (request.tuNgay != null)
            {
                filter &= Builders<PhieuThanhLy>.Filter.Gte(x => x.ngayLap, request.tuNgay.Value);
            }


            if (request.denNgay != null)
            {
                filter &= Builders<PhieuThanhLy>.Filter.Lte(x => x.ngayLap, request.denNgay.Value);
            }

            var projection = Builders<PhieuThanhLy>.Projection
               .Include(x => x.Id)
               .Include(x => x.tenPhieu)
               .Include(x => x.createdDate)
               .Include(x => x.ngayLap)
               .Include(x => x.diaDiem)
               .Include(x => x.ghiChu)
               .Include(x => x.nhanVien)
               .Include(x => x.loaiNguyenLieus);

            var findOptions = new FindOptions<PhieuThanhLy, PhieuThanhLy>
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
                var phieuThanhLys = await cursor.ToListAsync();

                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuThanhLys.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuThanhLys.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

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
                foreach (var phieuThanhLy in phieuThanhLys)
                {
                    var nguyenLieuIds = phieuThanhLy.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong)
                        .Include(x => x.hanSuDung)
                        .Include(x => x.donViTinh);
                    var newNguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                        .Project<NguyenLieu>(nguyenLieuProjection)
                        .ToListAsync();
                    var donViTinhIds = newNguyenLieus.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                    var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                    var donViTinhProjection = Builders<DonViTinh>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenDonViTinh);
                    var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                        .Project<DonViTinh>(donViTinhProjection)
                        .ToListAsync();
                    var uniqueNguyenLieus = newNguyenLieus.Where(x => !nguyenLieus.Any(y => y.Id == x.Id));

                    nguyenLieus.AddRange(uniqueNguyenLieus);

                    var newDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
                    var newDict2 = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                    foreach (var item in newDict)
                    {
                        if (!nguyenLieuDict.ContainsKey(item.Key))
                        {
                            nguyenLieuDict.Add(item.Key, item.Value);
                        }
                    }
                    foreach (var item in newDict2)
                    {
                        if (!donViTinhDict.ContainsKey(item.Key))
                        {
                            donViTinhDict.Add(item.Key, item.Value);
                        }
                    }
                }

                var phieuThanhLyResponds = phieuThanhLys.Select(x => new PhieuThanhLyRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayLap = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuThanhLyRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuThanhLyRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            hanSuDung = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.hanSuDung : null,
                            donViTinh = new IdName
                            {
                                Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                                Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                                ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                            },
                            soLuongBanDau = z.soLuongBanDau,
                            soLuongThanhLy = z.soLuongThanhLy,
                            chenhLech = z.chenhLech,
                            lyDoThanhLy = z.lyDoThanhLy
                        }).ToList()
                    }).ToList()
                }).ToList();
                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<PhieuThanhLyRespond>>
                {
                    Paging = pagingDetail,
                    Data = phieuThanhLyResponds
                };
                return new RespondAPIPaging<List<PhieuThanhLyRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var phieuThanhLys = await cursor.ToListAsync();

                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuThanhLys.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuThanhLys.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

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
                foreach (var phieuThanhLy in phieuThanhLys)
                {
                    var nguyenLieuIds = phieuThanhLy.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong)
                        .Include(x => x.hanSuDung)
                        .Include(x => x.donViTinh);
                    var newNguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                        .Project<NguyenLieu>(nguyenLieuProjection)
                        .ToListAsync();
                    var donViTinhIds = newNguyenLieus.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                    var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                    var donViTinhProjection = Builders<DonViTinh>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenDonViTinh);
                    var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                        .Project<DonViTinh>(donViTinhProjection)
                        .ToListAsync();
                    var uniqueNguyenLieus = newNguyenLieus.Where(x => !nguyenLieus.Any(y => y.Id == x.Id));

                    nguyenLieus.AddRange(uniqueNguyenLieus);

                    var newDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
                    var newDict2 = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                    foreach (var item in newDict)
                    {
                        if (!nguyenLieuDict.ContainsKey(item.Key))
                        {
                            nguyenLieuDict.Add(item.Key, item.Value);
                        }
                    }
                    foreach (var item in newDict2)
                    {
                        if (!donViTinhDict.ContainsKey(item.Key))
                        {
                            donViTinhDict.Add(item.Key, item.Value);
                        }
                    }
                }

                var phieuThanhLyResponds = phieuThanhLys.Select(x => new PhieuThanhLyRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayLap = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : null,
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuThanhLyRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuThanhLyRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            hanSuDung = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.hanSuDung : null,
                            donViTinh = new IdName
                            {
                                Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                                Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                                ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                            },
                            soLuongBanDau = z.soLuongBanDau,
                            soLuongThanhLy = z.soLuongThanhLy,
                            chenhLech = z.chenhLech,
                            lyDoThanhLy = z.lyDoThanhLy
                        }).ToList()
                    }).ToList()
                }).ToList();
                return new RespondAPIPaging<List<PhieuThanhLyRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhieuThanhLyRespond>>
                    {
                        Data = phieuThanhLyResponds,
                        Paging = new PagingDetail(1, phieuThanhLyResponds.Count, phieuThanhLyResponds.Count)
                    }
                );

            }

        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhieuThanhLyRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }

    }
    public async Task<RespondAPI<PhieuThanhLyRespond>> GetPhieuThanhLyById(string id)
    {
        try
        {

            var phieuThanhLy = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (phieuThanhLy == null)
            {
                return new RespondAPI<PhieuThanhLyRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu thanh lý với ID đã cung cấp."
                );
            }
            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = phieuThanhLy.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
            var nguyenLieuIds = phieuThanhLy.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
            var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNguyenLieu)
                .Include(x => x.soLuong)
                .Include(x => x.donViTinh);
            nguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                .Project<NguyenLieu>(nguyenLieuProjection)
                .ToListAsync();
            nguyenLieuDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
            var donViTinhIds = nguyenLieus.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
            var donViTinhProjection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh);
            var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                .Project<DonViTinh>(donViTinhProjection)
                .ToListAsync();
            donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
            var phieuThanhLyResponds = new PhieuThanhLyRespond
            {
                id = phieuThanhLy.Id,
                tenPhieu = phieuThanhLy.tenPhieu,
                ngayLap = phieuThanhLy.createdDate,
                diaDiem = phieuThanhLy.diaDiem,
                ghiChu = phieuThanhLy.ghiChu,
                nhanVien = phieuThanhLy.nhanVien != null ? new IdName
                {
                    Id = phieuThanhLy.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == phieuThanhLy.nhanVien).FirstOrDefault()?.tenNhanVien
                } : new IdName { Id = "", Name = "" },
                loaiNguyenLieus = phieuThanhLy.loaiNguyenLieus.Select(y => new loaiNguyenLieuThanhLyRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuThanhLyRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                        hanSuDung = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.hanSuDung : null,
                        donViTinh = new IdName
                        {
                            Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                            Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                            ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                        },
                        soLuongBanDau = z.soLuongBanDau,
                        soLuongThanhLy = z.soLuongThanhLy,
                        chenhLech = z.chenhLech,
                        lyDoThanhLy = z.lyDoThanhLy
                    }).ToList()
                }).ToList()
            };

            return new RespondAPI<PhieuThanhLyRespond>(
                ResultRespond.Succeeded,
                "Thành công.",
                phieuThanhLyResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuThanhLyRespond>(
                ResultRespond.Error,
                message: ex.Message
            );
        }

    }

    public async Task<RespondAPI<PhieuThanhLyRespond>> CreatePhieuThanhLy(RequestAddPhieuThanhLy request)
    {
        try
        {
            PhieuThanhLy newPhieuThanhLy = _mapper.Map<PhieuThanhLy>(request);
            newPhieuThanhLy.createdDate = DateTimeOffset.UtcNow;
            newPhieuThanhLy.updatedDate = DateTimeOffset.UtcNow;
            newPhieuThanhLy.isDelete = false;
            newPhieuThanhLy.ngayLap = newPhieuThanhLy.createdDate; 
            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = newPhieuThanhLy.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
            var nguyenLieuIds = newPhieuThanhLy.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
            var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNguyenLieu)
                .Include(x => x.soLuong)
                .Include(x => x.donViTinh);
            nguyenLieus = await _collectionNguyenLieu.Find(nguyenLieuFilter)
                .Project<NguyenLieu>(nguyenLieuProjection)
                .ToListAsync();
            nguyenLieuDict = nguyenLieus.ToDictionary(x => x.Id, x => x.tenNguyenLieu);
            foreach (var loai in newPhieuThanhLy.loaiNguyenLieus)
            {
                foreach (var nguyenLieu in loai.nguyenLieus)
                {
                    nguyenLieu.soLuongBanDau = nguyenLieus.FirstOrDefault(x => x.Id == nguyenLieu.id)?.soLuong ?? 0;
                }
            }

            foreach (var loai in request.loaiNguyenLieus)
            {
                foreach (var item in loai.nguyenLieus)
                {
                    var filter = Builders<NguyenLieu>.Filter.Eq(x => x.Id, item.id);
                    var update = Builders<NguyenLieu>.Update
                        .Set(x => x.soLuong, item.chenhLech)
                        .Set(x => x.updatedDate, DateTimeOffset.UtcNow);

                    await _collectionNguyenLieu.UpdateOneAsync(filter, update);
                }
            }

            await _collection.InsertOneAsync(newPhieuThanhLy);

            var donViTinhIds = nguyenLieus.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
            var donViTinhProjection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh);
            var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                .Project<DonViTinh>(donViTinhProjection)
                .ToListAsync();
            donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
            var phieuThanhLyResponds = new PhieuThanhLyRespond
            {
                id = newPhieuThanhLy.Id,
                tenPhieu = newPhieuThanhLy.tenPhieu,
                ngayLap = newPhieuThanhLy.createdDate,
                diaDiem = newPhieuThanhLy.diaDiem,
                ghiChu = newPhieuThanhLy.ghiChu,
                nhanVien = newPhieuThanhLy.nhanVien != null ? new IdName
                {
                    Id = newPhieuThanhLy.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == newPhieuThanhLy.nhanVien).FirstOrDefault()?.tenNhanVien
                } : null,
                loaiNguyenLieus = newPhieuThanhLy.loaiNguyenLieus.Select(y => new loaiNguyenLieuThanhLyRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuThanhLyRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                        hanSuDung = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.hanSuDung : null,
                        donViTinh = new IdName
                        {
                            Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                            Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                            ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                        },
                        soLuongBanDau = z.soLuongBanDau,
                        soLuongThanhLy = z.soLuongThanhLy,
                        chenhLech = z.chenhLech,
                        lyDoThanhLy = z.lyDoThanhLy
                    }).ToList()
                }).ToList()
            };
            return new RespondAPI<PhieuThanhLyRespond>(
                ResultRespond.Succeeded,
                "Tạo phiếu thanh lý thành công.",
                phieuThanhLyResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuThanhLyRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tìm kiếm phiếu thanh lý: {ex.Message}"
            );
        }

    }

    public async Task<RespondAPI<string>> DeletePhieuThanhLy(string id)
    {
        try
        {
            var existingPhieuThanhLy = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingPhieuThanhLy == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu lý để xóa."
                );
            }
            foreach (var loai in existingPhieuThanhLy.loaiNguyenLieus)
            {
                foreach (var item in loai.nguyenLieus)
                {
                    var filter = Builders<NguyenLieu>.Filter.Eq(x => x.Id, item.id);
                    var update = Builders<NguyenLieu>.Update
                        .Set(x => x.soLuong, item.chenhLech + item.soLuongThanhLy)
                        .Set(x => x.updatedDate, DateTimeOffset.UtcNow);

                    await _collectionNguyenLieu.UpdateOneAsync(filter, update);
                }
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa phiếu lý thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa phiếu thanh lý thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa thanh lý: {ex.Message}"
            );
        }
    }


}