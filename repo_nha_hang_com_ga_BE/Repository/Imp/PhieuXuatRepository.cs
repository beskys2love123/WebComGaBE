using System.Runtime.CompilerServices;
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
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuXuat;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuXuat;

namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhieuXuatRepository : IPhieuXuatRepository
{
    private readonly IMongoCollection<PhieuXuat> _collection;
    private readonly IMongoCollection<DonViTinh> _collectionDonViTinh;
    private readonly IMongoCollection<LoaiNguyenLieu> _collectionLoaiNguyenLieu;
    private readonly IMongoCollection<NguyenLieu> _collectionNguyenLieu;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;
    private readonly IMapper _mapper;

    public PhieuXuatRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhieuXuat>("PhieuXuat");
        _collectionLoaiNguyenLieu = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _collectionNguyenLieu = database.GetCollection<NguyenLieu>("NguyenLieu");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");
        _collectionDonViTinh = database.GetCollection<DonViTinh>("DonViTinh");
        _mapper = mapper;
    }
    public async Task<RespondAPIPaging<List<PhieuXuatRespond>>> GetAllPhieuXuats(RequestSearchPhieuXuat request)
    {
        try
        {
            var collection = _collection;
            var filter = Builders<PhieuXuat>.Filter.Empty;
            filter &= Builders<PhieuXuat>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhieu))
            {
                filter &= Builders<PhieuXuat>.Filter.Regex(x => x.tenPhieu, new BsonRegularExpression($".*{request.tenPhieu}.*"));
            }

            if (request.tuNgay != null)
            {
                filter &= Builders<PhieuXuat>.Filter.Gte(x => x.ngayLap, request.tuNgay.Value);
            }


            if (request.denNgay != null)
            {
                filter &= Builders<PhieuXuat>.Filter.Lte(x => x.ngayLap, request.denNgay.Value);
            }
            var projection = Builders<PhieuXuat>.Projection
               .Include(x => x.Id)
               .Include(x => x.tenPhieu)
               .Include(x => x.createdDate)
               .Include(x => x.ngayLap)
               .Include(x => x.nguoiNhan)
               .Include(x => x.lyDoXuat)
               .Include(x => x.nhanVien)
               .Include(x => x.diaDiem)
               .Include(x => x.ghiChu)
               .Include(x => x.loaiNguyenLieus);

            var findOptions = new FindOptions<PhieuXuat, PhieuXuat>
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
                var phieuXuats = await cursor.ToListAsync();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuXuats.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuXuats.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
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
                foreach (var phieuXuat in phieuXuats)
                {
                    var nguyenLieuIds = phieuXuat.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong)
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
                var phieuXuatResponds = phieuXuats.Select(x => new PhieuXuatRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayLap = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    lyDoXuat = x.lyDoXuat,
                    nguoiNhan = x.nguoiNhan,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuXuatRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuXuatRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            donViTinh = new IdName
                            {
                                Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                                Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                                ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                            },
                            soLuongBanDau = z.soLuongBanDau,
                            soLuongXuat = z.soLuongXuat,
                            chenhLech = z.chenhLech
                        }).ToList()
                    }).ToList()
                }).ToList();
                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<PhieuXuatRespond>>
                {
                    Paging = pagingDetail,
                    Data = phieuXuatResponds
                };
                return new RespondAPIPaging<List<PhieuXuatRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var phieuXuats = await cursor.ToListAsync();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var nguyenLieuDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();

                var loaiNguyenLieuIds = phieuXuats.SelectMany(x => x.loaiNguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();

                var nhanVienIds = phieuXuats.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
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
                foreach (var phieuXuat in phieuXuats)
                {
                    var nguyenLieuIds = phieuXuat.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nguyenLieuFilter = Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                    var nguyenLieuProjection = Builders<NguyenLieu>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNguyenLieu)
                        .Include(x => x.soLuong)
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
                var phieuXuatResponds = phieuXuats.Select(x => new PhieuXuatRespond
                {
                    id = x.Id,
                    tenPhieu = x.tenPhieu,
                    ngayLap = x.createdDate,
                    diaDiem = x.diaDiem,
                    ghiChu = x.ghiChu,
                    lyDoXuat = x.lyDoXuat,
                    nguoiNhan = x.nguoiNhan,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    loaiNguyenLieus = x.loaiNguyenLieus.Select(y => new loaiNguyenLieuXuatRespond
                    {
                        Id = y.id,
                        Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                        nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuXuatRespond
                        {
                            id = z.id,
                            tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                            soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                            donViTinh = new IdName
                            {
                                Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                                Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                                ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                            },
                            soLuongBanDau = z.soLuongBanDau,
                            soLuongXuat = z.soLuongXuat,
                            chenhLech = z.chenhLech
                        }).ToList()
                    }).ToList()
                }).ToList();

                return new RespondAPIPaging<List<PhieuXuatRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhieuXuatRespond>>
                    {
                        Data = phieuXuatResponds,
                        Paging = new PagingDetail(1, phieuXuatResponds.Count, phieuXuatResponds.Count)
                    }
                );


            }

        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhieuXuatRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }
    public async Task<RespondAPI<PhieuXuatRespond>> GetPhieuXuatById(string id)
    {
        try
        {

            var phieuXuat = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (phieuXuat == null)
            {
                return new RespondAPI<PhieuXuatRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phieu xuất với ID đã cung cấp."
                );
            }
            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = phieuXuat.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
            var nguyenLieuIds = phieuXuat.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
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
            var phieuXuatResponds = new PhieuXuatRespond
            {
                id = phieuXuat.Id,
                tenPhieu = phieuXuat.tenPhieu,
                ngayLap = phieuXuat.createdDate,
                diaDiem = phieuXuat.diaDiem,
                ghiChu = phieuXuat.ghiChu,
                lyDoXuat = phieuXuat.lyDoXuat,
                nguoiNhan = phieuXuat.nguoiNhan,
                nhanVien = phieuXuat.nhanVien != null ? new IdName
                {
                    Id = phieuXuat.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == phieuXuat.nhanVien).FirstOrDefault()?.tenNhanVien
                } : new IdName { Id = "", Name = "" },
                loaiNguyenLieus = phieuXuat.loaiNguyenLieus.Select(y => new loaiNguyenLieuXuatRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuXuatRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                        donViTinh = new IdName
                        {
                            Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                            Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                            ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                        },
                        soLuongBanDau = z.soLuongBanDau,
                        soLuongXuat = z.soLuongXuat,
                        chenhLech = z.chenhLech
                    }).ToList()
                }).ToList()
            };

            return new RespondAPI<PhieuXuatRespond>(
                ResultRespond.Succeeded,
                "Thành công.",
                phieuXuatResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuXuatRespond>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhieuXuatRespond>> CreatePhieuXuat(RequestAddPhieuXuat request)
    {
        try
        {
            PhieuXuat newPhieuXuat = _mapper.Map<PhieuXuat>(request);
            newPhieuXuat.createdDate = DateTimeOffset.UtcNow;
            newPhieuXuat.updatedDate = DateTimeOffset.UtcNow;
            newPhieuXuat.isDelete = false;
            newPhieuXuat.ngayLap = newPhieuXuat.createdDate; 

            var nguyenLieuDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();

            var loaiNguyenLieuIds = newPhieuXuat.loaiNguyenLieus.Select(x => x.id).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            List<NguyenLieu> nguyenLieus = new List<NguyenLieu>();
            var nguyenLieuIds = newPhieuXuat.loaiNguyenLieus.SelectMany(x => x.nguyenLieus.Select(y => y.id)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
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
            foreach (var loai in newPhieuXuat.loaiNguyenLieus)
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

            await _collection.InsertOneAsync(newPhieuXuat);

            var donViTinhIds = nguyenLieus.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
            var donViTinhProjection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh);
            var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                .Project<DonViTinh>(donViTinhProjection)
                .ToListAsync();
            donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
            var phieuXuatResponds = new PhieuXuatRespond
            {
                id = newPhieuXuat.Id,
                tenPhieu = newPhieuXuat.tenPhieu,
                ngayLap = newPhieuXuat.createdDate,
                diaDiem = newPhieuXuat.diaDiem,
                ghiChu = newPhieuXuat.ghiChu,
                lyDoXuat = newPhieuXuat.lyDoXuat,
                nguoiNhan = newPhieuXuat.nguoiNhan,
                nhanVien = newPhieuXuat.nhanVien != null ? new IdName
                {
                    Id = newPhieuXuat.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == newPhieuXuat.nhanVien).FirstOrDefault()?.tenNhanVien
                } : null,
                loaiNguyenLieus = newPhieuXuat.loaiNguyenLieus.Select(y => new loaiNguyenLieuXuatRespond
                {
                    Id = y.id,
                    Name = loaiNguyenLieuDict.ContainsKey(y.id) ? loaiNguyenLieuDict[y.id] : null,
                    nguyenLieus = y.nguyenLieus.Select(z => new nguyenLieuXuatRespond
                    {
                        id = z.id,
                        tenNguyenLieu = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieuDict[z.id] : null,
                        soLuong = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.soLuong : null,
                        donViTinh = new IdName
                        {
                            Id = nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null,
                            Name = donViTinhDict.ContainsKey(nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null)
                            ? donViTinhDict[nguyenLieuDict.ContainsKey(z.id) ? nguyenLieus.FirstOrDefault(m => m.Id == z.id)?.donViTinh : null] : null
                        },
                        soLuongBanDau = z.soLuongBanDau,
                        soLuongXuat = z.soLuongXuat,
                        chenhLech = z.chenhLech
                    }).ToList()
                }).ToList()
            };
            return new RespondAPI<PhieuXuatRespond>(
                ResultRespond.Succeeded,
                "Tạo phiếu xuất thành công.",
                phieuXuatResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuXuatRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tìm kiếm phiếu xuất: {ex.Message}"
            );
        }

    }

    public async Task<RespondAPI<string>> DeletePhieuXuat(string id)
    {
        try
        {
            var existingPhieuXuat = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingPhieuXuat == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu xuất để xóa."
                );
            }
            foreach (var loai in existingPhieuXuat.loaiNguyenLieus)
            {
                foreach (var item in loai.nguyenLieus)
                {
                    var filter = Builders<NguyenLieu>.Filter.Eq(x => x.Id, item.id);
                    var update = Builders<NguyenLieu>.Update
                        .Set(x => x.soLuong, item.chenhLech + item.soLuongXuat)
                        .Set(x => x.updatedDate, DateTimeOffset.UtcNow);

                    await _collectionNguyenLieu.UpdateOneAsync(filter, update);
                }
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa phiếu xuất thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa phiếu xuất thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa xuất: {ex.Message}"
            );
        }
    }


}