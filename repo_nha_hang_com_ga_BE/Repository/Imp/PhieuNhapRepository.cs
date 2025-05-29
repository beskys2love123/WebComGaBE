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
using repo_nha_hang_com_ga_BE.Models.Requests.BaoCaoThongKe;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuNhap;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuNhap;
namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class PhieuNhapRepository : IPhieuNhapRepository
{
    private readonly IMongoCollection<PhieuNhap> _collection;
    private readonly IMongoCollection<NguyenLieu> _collectionNguyenLieu;
    private readonly IMongoCollection<LoaiNguyenLieu> _collectionLoaiNguyenLieu;
    private readonly IMongoCollection<DonViTinh> _collectionDonViTinh;
    private readonly IMongoCollection<TuDo> _collectionTuDo;
    private readonly IMongoCollection<NhaCungCap> _collectionNhaCungCap;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;
    private readonly IMapper _mapper;

    public PhieuNhapRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<PhieuNhap>("PhieuNhap");
        _collectionNguyenLieu = database.GetCollection<NguyenLieu>("NguyenLieu");
        _collectionLoaiNguyenLieu = database.GetCollection<LoaiNguyenLieu>("LoaiNguyenLieu");
        _collectionDonViTinh = database.GetCollection<DonViTinh>("DonViTinh");
        _collectionTuDo = database.GetCollection<TuDo>("TuDo");
        _collectionNhaCungCap = database.GetCollection<NhaCungCap>("NhaCungCap");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");

        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<PhieuNhapRespond>>> GetAllPhieuNhaps(RequestSearchPhieuNhap request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<PhieuNhap>.Filter.Empty;
            filter &= Builders<PhieuNhap>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenPhieu))
            {
                filter &= Builders<PhieuNhap>.Filter.Regex(x => x.tenPhieu, new BsonRegularExpression($".*{request.tenPhieu}.*"));
            }
            if (request.tuNgay != null)
            {
                filter &= Builders<PhieuNhap>.Filter.Gte(x => x.ngayLap, request.tuNgay.Value);
            }


            if (request.denNgay != null)
            {
                filter &= Builders<PhieuNhap>.Filter.Lte(x => x.ngayLap, request.denNgay.Value);
            }

            var projection = Builders<PhieuNhap>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenPhieu)
                .Include(x => x.tenNguoiGiao)
                .Include(x => x.createdDate)
                .Include(x => x.ngayLap)
                .Include(x => x.nhaCungCap)
                .Include(x => x.dienGiai)
                .Include(x => x.diaDiem)
                .Include(x => x.tongTien)
                .Include(x => x.ghiChu)
                .Include(x => x.nhanVien)
                .Include(x => x.nguyenLieus);

            var findOptions = new FindOptions<PhieuNhap, PhieuNhap>
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
                var PhieuNhaps = await cursor.ToListAsync();
                var nhaCungCapDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();
                var tuDoDict = new Dictionary<string, string>();


                var tuDoIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.tuDo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.loaiNguyenLieu)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var donViTinhIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.donViTinh)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhaCungCapIds = PhieuNhaps.Select(x => x.nhaCungCap).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhanVienIds = PhieuNhaps.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
                var nhaCungCapFilter = Builders<NhaCungCap>.Filter.In(x => x.Id, nhaCungCapIds);
                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var donViTinhProjection = Builders<DonViTinh>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenDonViTinh);
                var tuDoProjection = Builders<TuDo>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenTuDo);
                var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhaCungCap);
                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);

                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

                var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                    .Project<DonViTinh>(donViTinhProjection)
                    .ToListAsync();
                donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                var tuDos = await _collectionTuDo.Find(tuDoFilter)
                    .Project<TuDo>(tuDoProjection)
                    .ToListAsync();
                tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);
                var nhaCungCaps = await _collectionNhaCungCap.Find(nhaCungCapFilter)
                    .Project<NhaCungCap>(nhaCungCapProjection)
                    .ToListAsync();
                nhaCungCapDict = nhaCungCaps.ToDictionary(x => x.Id, x => x.tenNhaCungCap);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);

                var phieuNhapResponds = PhieuNhaps.Select(x => new PhieuNhapRespond
                {
                    id = x.Id,
                    ngayLap = x.createdDate,
                    tenPhieu = x.tenPhieu,
                    tenNguoiGiao = x.tenNguoiGiao,
                    nhaCungCap = x.nhaCungCap != null ? new IdName
                    {
                        Id = x.nhaCungCap,
                        Name = nhaCungCapDict.ContainsKey(x.nhaCungCap) ? nhaCungCapDict[x.nhaCungCap] : null
                    } : new IdName { Id = "", Name = "" },
                    dienGiai = x.dienGiai,
                    diaDiem = x.diaDiem,
                    tongTien = x.tongTien,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    nguyenLieus = x.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                    {
                        id = y.id,
                        tenNguyenLieu = y.tenNguyenLieu,
                        moTa = y.moTa,
                        soLuong = y.soLuong,
                        hanSuDung = y.hanSuDung,
                        donGia = y.donGia != null ? y.donGia : null,
                        thanhTien = y.thanhTien != null ? y.thanhTien : null,
                        loaiNguyenLieu = y.loaiNguyenLieu != null ? new IdName
                        {
                            Id = y.loaiNguyenLieu,
                            Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                        } : null,
                        donViTinh = y.donViTinh != null ? new IdName
                        {
                            Id = y.donViTinh,
                            Name = donViTinhDict.GetValueOrDefault(y.donViTinh)
                        } : null,
                        tuDo = y.tuDo != null ? new IdName
                        {
                            Id = y.tuDo,
                            Name = tuDoDict.GetValueOrDefault(y.tuDo)
                        } : null,
                        trangThai = y.trangThai
                    }).ToList()
                }).ToList();

                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<PhieuNhapRespond>>
                {
                    Paging = pagingDetail,
                    Data = phieuNhapResponds
                };

                return new RespondAPIPaging<List<PhieuNhapRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var PhieuNhaps = await cursor.ToListAsync();
                var nhaCungCapDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();
                var loaiNguyenLieuDict = new Dictionary<string, string>();
                var donViTinhDict = new Dictionary<string, string>();
                var tuDoDict = new Dictionary<string, string>();


                var tuDoIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.tuDo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.loaiNguyenLieu)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var donViTinhIds = PhieuNhaps.SelectMany(x => x.nguyenLieus.Select(y => y.donViTinh)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhaCungCapIds = PhieuNhaps.Select(x => x.nhaCungCap).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var nhanVienIds = PhieuNhaps.Select(x => x.nhanVien).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
                var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
                var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
                var nhaCungCapFilter = Builders<NhaCungCap>.Filter.In(x => x.Id, nhaCungCapIds);
                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);
                var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoai);
                var donViTinhProjection = Builders<DonViTinh>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenDonViTinh);
                var tuDoProjection = Builders<TuDo>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenTuDo);
                var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhaCungCap);
                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);

                var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                    .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                    .ToListAsync();
                loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

                var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                    .Project<DonViTinh>(donViTinhProjection)
                    .ToListAsync();
                donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
                var tuDos = await _collectionTuDo.Find(tuDoFilter)
                    .Project<TuDo>(tuDoProjection)
                    .ToListAsync();
                tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);
                var nhaCungCaps = await _collectionNhaCungCap.Find(nhaCungCapFilter)
                    .Project<NhaCungCap>(nhaCungCapProjection)
                    .ToListAsync();
                nhaCungCapDict = nhaCungCaps.ToDictionary(x => x.Id, x => x.tenNhaCungCap);
                var nhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();
                nhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);

                var phieuNhapResponds = PhieuNhaps.Select(x => new PhieuNhapRespond
                {
                    id = x.Id,
                    ngayLap = x.createdDate,
                    tenPhieu = x.tenPhieu,
                    tenNguoiGiao = x.tenNguoiGiao,
                    nhaCungCap = x.nhaCungCap != null ? new IdName
                    {
                        Id = x.nhaCungCap,
                        Name = nhaCungCapDict.ContainsKey(x.nhaCungCap) ? nhaCungCapDict[x.nhaCungCap] : null
                    } : new IdName { Id = "", Name = "" },
                    dienGiai = x.dienGiai,
                    diaDiem = x.diaDiem,
                    tongTien = x.tongTien,
                    ghiChu = x.ghiChu,
                    nhanVien = x.nhanVien != null ? new IdName
                    {
                        Id = x.nhanVien,
                        Name = nhanVienDict.ContainsKey(x.nhanVien) ? nhanVienDict[x.nhanVien] : null
                    } : new IdName { Id = "", Name = "" },
                    nguyenLieus = x.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                    {
                        id = y.id,
                        tenNguyenLieu = y.tenNguyenLieu,
                        moTa = y.moTa,
                        soLuong = y.soLuong,
                        hanSuDung = y.hanSuDung,
                        donGia = y.donGia != null ? y.donGia : null,
                        thanhTien = y.thanhTien != null ? y.thanhTien : null,
                        loaiNguyenLieu = y.loaiNguyenLieu != null ? new IdName
                        {
                            Id = y.loaiNguyenLieu,
                            Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                        } : null,
                        donViTinh = y.donViTinh != null ? new IdName
                        {
                            Id = y.donViTinh,
                            Name = donViTinhDict.GetValueOrDefault(y.donViTinh)
                        } : null,
                        tuDo = y.tuDo != null ? new IdName
                        {
                            Id = y.tuDo,
                            Name = tuDoDict.GetValueOrDefault(y.tuDo)
                        } : null,
                        trangThai = y.trangThai
                    }).ToList()
                }).ToList();

                return new RespondAPIPaging<List<PhieuNhapRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<PhieuNhapRespond>>
                    {
                        Data = phieuNhapResponds,
                        Paging = new PagingDetail(1, phieuNhapResponds.Count, phieuNhapResponds.Count)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<PhieuNhapRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhieuNhapRespond>> GetPhieuNhapById(string id)
    {
        try
        {

            var phieuNhap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (phieuNhap == null)
            {
                return new RespondAPI<PhieuNhapRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phieu nhập với ID đã cung cấp."
                );
            }

            var phieuNhapResponds = new PhieuNhapRespond
            {
                id = phieuNhap.Id,
                ngayLap = phieuNhap.createdDate,
                tenPhieu = phieuNhap.tenPhieu,
                tenNguoiGiao = phieuNhap.tenNguoiGiao,
                nhaCungCap = phieuNhap.nhaCungCap != null ? new IdName
                {
                    Id = phieuNhap.nhaCungCap,
                    Name = _collectionNhaCungCap.Find(x => x.Id == phieuNhap.nhaCungCap).FirstOrDefault()?.tenNhaCungCap,
                } : new IdName { Id = "", Name = "" },
                dienGiai = phieuNhap.dienGiai,
                diaDiem = phieuNhap.diaDiem,
                tongTien = phieuNhap.tongTien,
                ghiChu = phieuNhap.ghiChu,
                nhanVien = phieuNhap.nhanVien != null ? new IdName
                {
                    Id = phieuNhap.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == phieuNhap.nhanVien).FirstOrDefault()?.tenNhanVien,
                } : new IdName { Id = "", Name = "" },
                nguyenLieus = phieuNhap.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                {
                    id = y.id,
                    tenNguyenLieu = y.tenNguyenLieu,
                    moTa = y.moTa,
                    soLuong = y.soLuong,
                    hanSuDung = y.hanSuDung,
                    donGia = y.donGia != null ? y.donGia : null,
                    thanhTien = y.thanhTien != null ? y.thanhTien : null,
                    loaiNguyenLieu = y.loaiNguyenLieu != null ? new IdName
                    {
                        Id = y.loaiNguyenLieu,
                        Name = _collectionLoaiNguyenLieu.Find(x => x.Id == y.loaiNguyenLieu).FirstOrDefault()?.tenLoai
                    } : null,
                    donViTinh = y.donViTinh != null ? new IdName
                    {
                        Id = y.donViTinh,
                        Name = _collectionDonViTinh.Find(x => x.Id == y.donViTinh).FirstOrDefault()?.tenDonViTinh
                    } : null,
                    tuDo = y.tuDo != null ? new IdName
                    {
                        Id = y.tuDo,
                        Name = _collectionTuDo.Find(x => x.Id == y.tuDo).FirstOrDefault()?.tenTuDo
                    } : null,
                    trangThai = y.trangThai
                }).ToList()
            };

            return new RespondAPI<PhieuNhapRespond>(
            ResultRespond.Succeeded,
            "Lấy phiếu nhập thành công.",
            phieuNhapResponds
        );


        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<PhieuNhapRespond>> CreatePhieuNhap(RequestAddPhieuNhap request)
    {
        try
        {

            var nguyenLieuEntities = request.nguyenLieus.Select(nl =>
            {
                var entity = new NguyenLieu();
                entity.tenNguyenLieu = nl.tenNguyenLieu;
                entity.moTa = nl.moTa;
                entity.hanSuDung = nl.hanSuDung;
                entity.soLuong = nl.soLuong;
                entity.loaiNguyenLieu = nl.loaiNguyenLieu;
                entity.donViTinh = nl.donViTinh;
                entity.tuDo = nl.tuDo;
                entity.trangThai = nl.trangThai;
                entity.createdDate = DateTimeOffset.UtcNow;
                entity.updatedDate = DateTimeOffset.UtcNow;
                entity.isDelete = false;
                return entity;
            }).ToList();


            await _collectionNguyenLieu.InsertManyAsync(nguyenLieuEntities);

            var nguyenLieuIds = nguyenLieuEntities.Select(x => x.Id).ToList();

            var phieuNhap = new PhieuNhap();
            phieuNhap.tenPhieu = request.tenPhieu;
            phieuNhap.ngayLap = DateTimeOffset.UtcNow;
            phieuNhap.tenNguoiGiao = request.tenNguoiGiao;
            phieuNhap.nhaCungCap = request.nhaCungCap;
            phieuNhap.dienGiai = request.dienGiai;
            phieuNhap.diaDiem = request.diaDiem;
            phieuNhap.tongTien = request.tongTien;
            phieuNhap.nguyenLieus = nguyenLieuEntities.Select(x => new nguyenLieuMenu
            {
                id = x.Id,
                tenNguyenLieu = x.tenNguyenLieu,
                hanSuDung = request.nguyenLieus.Where(y => y.tenNguyenLieu == x.tenNguyenLieu).FirstOrDefault()?.hanSuDung,
                moTa = x.moTa,
                soLuong = x.soLuong,
                loaiNguyenLieu = x.loaiNguyenLieu,
                donViTinh = x.donViTinh,
                tuDo = x.tuDo,
                trangThai = x.trangThai,
                thanhTien = request.nguyenLieus.Where(y => y.tenNguyenLieu == x.tenNguyenLieu).FirstOrDefault()?.thanhTien,
                donGia = request.nguyenLieus.Where(y => y.tenNguyenLieu == x.tenNguyenLieu).FirstOrDefault()?.donGia
            }).ToList();
            phieuNhap.nhanVien = request.nhanVien;
            phieuNhap.ghiChu = request.ghiChu;
            phieuNhap.createdDate = DateTimeOffset.UtcNow;
            phieuNhap.updatedDate = DateTimeOffset.UtcNow;
            phieuNhap.isDelete = false;


            PhieuNhap newPhieuNhap = _mapper.Map<PhieuNhap>(phieuNhap);
            newPhieuNhap.createdDate = DateTimeOffset.UtcNow;
            newPhieuNhap.updatedDate = DateTimeOffset.UtcNow;
            newPhieuNhap.isDelete = false;
            newPhieuNhap.ngayLap = newPhieuNhap.createdDate;





            await _collection.InsertOneAsync(newPhieuNhap);

            var nhaCungCapDict = new Dictionary<string, string>();
            var nhanVienDict = new Dictionary<string, string>();
            var loaiNguyenLieuDict = new Dictionary<string, string>();
            var nguyenLieuDict = new Dictionary<string, string>();
            var donViTinhDict = new Dictionary<string, string>();
            var tuDoDict = new Dictionary<string, string>();


            var tuDoIds = nguyenLieuEntities.Select(x => x.tuDo).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var loaiNguyenLieuIds = nguyenLieuEntities.Select(x => x.loaiNguyenLieu).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var donViTinhIds = nguyenLieuEntities.Select(x => x.donViTinh).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var nhaCungCapIds = newPhieuNhap.nhaCungCap;
            var nhanVienIds = newPhieuNhap.nhanVien;
            var loaiNguyenLieuFilter = Builders<LoaiNguyenLieu>.Filter.In(x => x.Id, loaiNguyenLieuIds);
            var donViTinhFilter = Builders<DonViTinh>.Filter.In(x => x.Id, donViTinhIds);
            var tuDoFilter = Builders<TuDo>.Filter.In(x => x.Id, tuDoIds);
            var loaiNguyenLieuProjection = Builders<LoaiNguyenLieu>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoai);
            var donViTinhProjection = Builders<DonViTinh>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDonViTinh);
            var tuDoProjection = Builders<TuDo>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenTuDo);
            var nhaCungCapProjection = Builders<NhaCungCap>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNhaCungCap);
            var nhanVienProjection = Builders<NhanVien>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenNhanVien);

            var loaiNguyenLieus = await _collectionLoaiNguyenLieu.Find(loaiNguyenLieuFilter)
                .Project<LoaiNguyenLieu>(loaiNguyenLieuProjection)
                .ToListAsync();
            loaiNguyenLieuDict = loaiNguyenLieus.ToDictionary(x => x.Id, x => x.tenLoai);

            var donViTinhs = await _collectionDonViTinh.Find(donViTinhFilter)
                .Project<DonViTinh>(donViTinhProjection)
                .ToListAsync();
            donViTinhDict = donViTinhs.ToDictionary(x => x.Id, x => x.tenDonViTinh);
            var tuDos = await _collectionTuDo.Find(tuDoFilter)
                .Project<TuDo>(tuDoProjection)
                .ToListAsync();
            tuDoDict = tuDos.ToDictionary(x => x.Id, x => x.tenTuDo);

            var phieuNhapResponds = new PhieuNhapRespond
            {
                id = newPhieuNhap.Id,
                tenPhieu = newPhieuNhap.tenPhieu,
                ngayLap=newPhieuNhap.ngayLap,
                tenNguoiGiao = newPhieuNhap.tenNguoiGiao,
                nhaCungCap = newPhieuNhap.nhaCungCap != null ? new IdName
                {
                    Id = newPhieuNhap.nhaCungCap,
                    Name = _collectionNhaCungCap.Find(x => x.Id == newPhieuNhap.nhaCungCap).FirstOrDefault()?.tenNhaCungCap,
                } : null,
                dienGiai = newPhieuNhap.dienGiai,
                diaDiem = newPhieuNhap.diaDiem,
                tongTien = newPhieuNhap.tongTien,
                ghiChu = newPhieuNhap.ghiChu,
                nhanVien = newPhieuNhap.nhanVien != null ? new IdName
                {
                    Id = newPhieuNhap.nhanVien,
                    Name = _collectionNhanVien.Find(x => x.Id == newPhieuNhap.nhanVien).FirstOrDefault()?.tenNhanVien,
                } : null,
                nguyenLieus = newPhieuNhap.nguyenLieus.Select(y => new nguyenLieuMenuRespond
                {
                    id = y.id,
                    tenNguyenLieu = y.tenNguyenLieu,
                    moTa = y.moTa,
                    soLuong = y.soLuong,
                    hanSuDung = y.hanSuDung,
                    donGia = y.donGia != null ? y.donGia : null,
                    thanhTien = y.thanhTien != null ? y.thanhTien : null,
                    loaiNguyenLieu = y.loaiNguyenLieu != null ? new IdName
                    {
                        Id = y.loaiNguyenLieu,
                        Name = loaiNguyenLieuDict.ContainsKey(y.loaiNguyenLieu) ? loaiNguyenLieuDict[y.loaiNguyenLieu] : null
                    } : null,
                    donViTinh = y.donViTinh != null ? new IdName
                    {
                        Id = y.donViTinh,
                        Name = donViTinhDict.GetValueOrDefault(y.donViTinh) ?? ""
                    } : null,
                    tuDo = y.tuDo != null ? new IdName
                    {
                        Id = y.tuDo,
                        Name = tuDoDict.GetValueOrDefault(y.tuDo) ?? ""
                    } : null,
                    trangThai = y.trangThai
                }).ToList()
            };

            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Succeeded,
                "Tạo phiếu nhập thành công.",
                phieuNhapResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<PhieuNhapRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo phiếu nhập: {ex.Message}"
            );
        }

    }

    public async Task<RespondAPI<string>> DeletePhieuNhap(string id)
    {
        try
        {
            var existingPhieuNhap = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingPhieuNhap == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy phiếu nhập để xóa."
                );
            }
            var nguyenLieuIds = existingPhieuNhap.nguyenLieus.Select(nl => nl.id).Where(nlid => !string.IsNullOrEmpty(nlid)).Distinct().ToList();

            if (nguyenLieuIds.Any())
            {

                var deleteNguyenLieuFilter =
                    Builders<NguyenLieu>.Filter.In(x => x.Id, nguyenLieuIds);
                await _collectionNguyenLieu.DeleteManyAsync(deleteNguyenLieuFilter);
            }
            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa phiếu nhập không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa phiếu thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa phiếu nhập: {ex.Message}"
            );
        }
    }

    public async Task<List<KhoanChiRespond>> GetKhoanChi(RequestSearchThoiGian request)
    {
        try
        {
            var filter = Builders<PhieuNhap>.Filter.Empty;
            filter &= Builders<PhieuNhap>.Filter.Eq(x => x.isDelete, false);
            if (request.doanhThuEnum == DoanhThuEnum.TheoNgay || request.doanhThuEnum == DoanhThuEnum.TheoThang)
            {
                if (request.tuNgay != null)
                {
                    filter &= Builders<PhieuNhap>.Filter.Gte(x => x.createdDate, request.tuNgay.Value);
                }
                if (request.denNgay != null)
                {
                    filter &= Builders<PhieuNhap>.Filter.Lte(x => x.createdDate, request.denNgay.Value);
                }
            }
            else if (request.doanhThuEnum == DoanhThuEnum.TheoTuan)
            {
                if (request.tuNgay != null)
                {
                    filter &= Builders<PhieuNhap>.Filter.Gte(x => x.createdDate, request.tuNgay.Value);
                }
            }

            var phieuNhaps = await _collection.Find(filter).ToListAsync();
            var khoanChiResponds = new List<KhoanChiRespond>();


            var phieuNhapDict = phieuNhaps.ToDictionary(x => x.Id, x => x.tongTien);


            if (request.doanhThuEnum == DoanhThuEnum.TheoNgay)
            {

                var startDate = request.tuNgay.Value.Date;
                var endDate = request.denNgay.Value.Date;
                var dailyRevenue = new List<KhoanChiRespond>();

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayOrders = phieuNhaps
                        .Where(x => x.createdDate?.Date == date)
                        .ToList();

                    dailyRevenue.Add(new KhoanChiRespond
                    {
                        thoiGian = date.ToString("dd/MM/yyyy"),
                        khoanChi = dayOrders.Sum(x => phieuNhapDict.ContainsKey(x.Id) ? phieuNhapDict[x.Id] : 0)
                    });
                }

                return dailyRevenue;
            }
            else if (request.doanhThuEnum == DoanhThuEnum.TheoTuan)
            {

                var startDate = request.tuNgay.Value.Date;
                var weeklyRevenue = new List<KhoanChiRespond>();
                var numberOfWeeks = request.soTuan ?? 4;

                for (int i = 0; i < numberOfWeeks; i++)
                {
                    var weekStart = startDate.AddDays(i * 7);
                    var weekEnd = weekStart.AddDays(6);

                    var weekOrders = phieuNhaps
                        .Where(x => x.createdDate?.Date >= weekStart && x.createdDate?.Date <= weekEnd)
                        .ToList();

                    weeklyRevenue.Add(new KhoanChiRespond
                    {
                        thoiGian = $"Tuần {i + 1} ({weekStart:dd/MM/yyyy} - {weekEnd:dd/MM/yyyy})",
                        khoanChi = weekOrders.Sum(x => phieuNhapDict.ContainsKey(x.Id) ? phieuNhapDict[x.Id] : 0)
                    });
                }

                return weeklyRevenue;
            }
            else if (request.doanhThuEnum == DoanhThuEnum.TheoThang)
            {

                var startDate = request.tuNgay.Value.Date;
                var endDate = request.denNgay.Value.Date;
                var monthlyRevenue = new List<KhoanChiRespond>();

                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    var monthStart = new DateTime(date.Year, date.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthOrders = phieuNhaps
                        .Where(x => x.createdDate?.Date >= monthStart && x.createdDate?.Date <= monthEnd)
                        .ToList();

                    monthlyRevenue.Add(new KhoanChiRespond
                    {
                        thoiGian = $"{date.Month}/{date.Year}",
                        khoanChi = monthOrders.Sum(x => phieuNhapDict.ContainsKey(x.Id) ? phieuNhapDict[x.Id] : 0)
                    });
                }

                return monthlyRevenue;
            }

            return khoanChiResponds;
        }
        catch (Exception ex)
        {
            return new List<KhoanChiRespond>();
        }
    }

}