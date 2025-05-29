
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using repo_nha_hang_com_ga_BE.Models.Common;
using repo_nha_hang_com_ga_BE.Models.Common.Models;
using repo_nha_hang_com_ga_BE.Models.Common.Models.Respond;
using repo_nha_hang_com_ga_BE.Models.Common.Paging;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.MongoDB;
using repo_nha_hang_com_ga_BE.Models.Requests.DonOrder;
using repo_nha_hang_com_ga_BE.Models.Responds.DonOrder;


namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class DonOrderRepository : IDonOrderRepository
{
    private readonly IMongoCollection<DonOrder> _collection;
    private readonly IMongoCollection<Ban> _collectionBan;
    private readonly IMongoCollection<LoaiDon> _collectionLoaiDon;
    private readonly IMongoCollection<MonAn> _collectionMonAn;
    private readonly IMongoCollection<KhachHang> _collectionkhachHang;
    private readonly IMongoCollection<Combo> _collectionCombo;

    private readonly IMapper _mapper;

    public DonOrderRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<DonOrder>("DonOrder");
        _collectionBan = database.GetCollection<Ban>("Ban");
        _collectionLoaiDon = database.GetCollection<LoaiDon>("LoaiDon");
        _collectionMonAn = database.GetCollection<MonAn>("MonAn");
        _collectionkhachHang = database.GetCollection<KhachHang>("KhachHang");
        _collectionCombo = database.GetCollection<Combo>("Combo");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<DonOrderRespond>>> GetAllDonOrder(RequestSearchDonOrder request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<DonOrder>.Filter.Empty;
            filter &= Builders<DonOrder>.Filter.Eq(x => x.isDelete, false);

            if (!string.IsNullOrEmpty(request.tenDon))
            {
                filter &= Builders<DonOrder>.Filter.Regex(x => x.tenDon, request.tenDon);
            }

            if (!string.IsNullOrEmpty(request.loaiDon))
            {
                filter &= Builders<DonOrder>.Filter.Regex(x => x.loaiDon, request.loaiDon);
            }

            if (!string.IsNullOrEmpty(request.ban))
            {
                filter &= Builders<DonOrder>.Filter.Eq(x => x.ban, request.ban);
            }
            if (request.khachHang != null && request.khachHang.Any())
            {
                var khachHangFilters = request.khachHang.Select(kh =>
                    Builders<DonOrder>.Filter.Eq(x => x.khachHang, kh));
                filter &= Builders<DonOrder>.Filter.Or(khachHangFilters);
            }

            if (request.trangThai.HasValue)
            {
                filter &= Builders<DonOrder>.Filter.Eq(x => x.trangThai, request.trangThai);
            }

            var projection = Builders<DonOrder>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenDon)
                .Include(x => x.loaiDon)
                .Include(x => x.ban)
                .Include(x => x.khachHang)
                .Include(x => x.trangThai)
                .Include(x => x.chiTietDonOrder)
                .Include(x => x.tongTien)
                .Include(x => x.createdDate);

            var findOptions = new FindOptions<DonOrder, DonOrder>
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
                var dons = await cursor.ToListAsync();


                var banIds = dons.Select(x => x.ban).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


                var loaiDonIds = dons.Select(x => x.loaiDon).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


                var khachHangIds = dons.Select(x => x.khachHang).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();



                var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
                var banProjection = Builders<Ban>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenBan);
                var bans = await _collectionBan.Find(banFilter)
                    .Project<Ban>(banProjection)
                    .ToListAsync();


                var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
                var loaiDonProjection = Builders<LoaiDon>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoaiDon);
                var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
                    .Project<LoaiDon>(loaiDonProjection)
                    .ToListAsync();


                var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
                var khachHangProjection = Builders<KhachHang>.Projection
                   .Include(x => x.Id)
                   .Include(x => x.tenKhachHang);
                var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
                  .Project<KhachHang>(khachHangProjection)
                  .ToListAsync();



                var banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);
                var loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);
                var monAnDict = new Dictionary<string, string>();
                var khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);
                var comBoDict = new Dictionary<string, string>();

                List<MonAn> monAns = new List<MonAn>();
                foreach (var don in dons)
                {
                    var monAnIds = dons.SelectMany(x => x.chiTietDonOrder)
                        .SelectMany(ct => ct.monAns)
                        .Select(ma => ma.monAn)
                        .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);

                    var monAnProjection = Builders<MonAn>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenMonAn)
                        .Include(x => x.giaTien);

                    var newMonAns = await _collectionMonAn.Find(monAnFilter)
                        .Project<MonAn>(monAnProjection)
                        .ToListAsync();

                    var uniqueMonAns = newMonAns.Where(x => !monAns.Any(y => y.Id == x.Id));
                    monAns.AddRange(uniqueMonAns);

                    var newDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
                    foreach (var item in newDict)
                    {
                        if (!monAnDict.ContainsKey(item.Key))
                        {
                            monAnDict.Add(item.Key, item.Value);
                        }
                    }
                }

                List<Combo> comBos = new List<Combo>();
                foreach (var don in dons)
                {
                    var comBoIds = dons.SelectMany(x => x.chiTietDonOrder)
                       .SelectMany(ct => ct.comBos)
                       .Select(ma => ma.comBo)
                       .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);

                    var comBoProjection = Builders<Combo>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenCombo)
                        .Include(x => x.giaTien);

                    var newComBos = await _collectionCombo.Find(comBoFilter)
                    .Project<Combo>(comBoProjection)
                    .ToListAsync();

                    var uniqueComBos = newComBos.Where(x => !comBos.Any(y => y.Id == x.Id));
                    comBos.AddRange(uniqueComBos);

                    var newComBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);
                    foreach (var item in newComBoDict)
                    {
                        if (!comBoDict.ContainsKey(item.Key))
                        {
                            comBoDict.Add(item.Key, item.Value);
                        }
                    }
                }



                var donOrderResponds = dons.Select(donOrder => new DonOrderRespond
                {
                    id = donOrder.Id,
                    tenDon = donOrder.tenDon,
                    loaiDon = new IdName
                    {
                        Id = donOrder.loaiDon,
                        Name = donOrder.loaiDon != null && loaiDonDict.ContainsKey(donOrder.loaiDon) ? loaiDonDict[donOrder.loaiDon] : null
                    },
                    ban = new IdName
                    {
                        Id = donOrder.ban,
                        Name = donOrder.ban != null && banDict.ContainsKey(donOrder.ban) ? banDict[donOrder.ban] : null
                    },
                    khachHang = new IdName
                    {
                        Id = donOrder.khachHang,
                        Name = donOrder.khachHang != null && khachHangDict.ContainsKey(donOrder.khachHang) ? khachHangDict[donOrder.khachHang] : null
                    },
                    chiTietDonOrder = donOrder.chiTietDonOrder.Select(ct => new ChiTietDonOrderRespond
                    {
                        monAns = ct.monAns.Select(ma => new DonMonAnRespond
                        {
                            monAn = new IdName
                            {
                                Id = ma.monAn,
                                Name = ma.monAn != null && monAnDict.ContainsKey(ma.monAn) ? monAnDict[ma.monAn] : null
                            },

                            monAn_trangThai = ma.monAn_trangThai,
                            soLuong = ma.soLuong,
                            giaTien = monAnDict.ContainsKey(ma.monAn) ? monAns.FirstOrDefault(m => m.Id == ma.monAn)?.giaTien : null,
                            moTa = ma.moTa,
                        }).ToList(),
                        comBos = ct.comBos.Select(ma => new DonComBoRespond
                        {
                            comBo = new IdName
                            {
                                Id = ma.comBo,
                                Name = ma.comBo != null && comBoDict.ContainsKey(ma.comBo) ? comBoDict[ma.comBo] : null
                            },
                            comBo_trangThai = ma.comBo_trangThai,
                            soLuong = ma.soLuong,
                            giaTien = comBoDict.ContainsKey(ma.comBo) ? comBos.FirstOrDefault(m => m.Id == ma.comBo)?.giaTien : null,
                            moTa = ma.moTa,
                        }).ToList(),
                        trangThai = ct.trangThai,
                    }).ToList(),
                    trangThai = donOrder.trangThai,
                    tongTien = donOrder.tongTien,
                    createdDate = donOrder.createdDate?.Date,
                    ngayTao = donOrder.createdDate,
                }).OrderBy(x => x.trangThai)
                    .ThenByDescending(x => x.ngayTao).ToList();
                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<DonOrderRespond>>
                {
                    Paging = pagingDetail,
                    Data = donOrderResponds
                };

                return new RespondAPIPaging<List<DonOrderRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var dons = await cursor.ToListAsync();


                var banIds = dons.Select(x => x.ban).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


                var loaiDonIds = dons.Select(x => x.loaiDon).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


                var khachHangIds = dons.Select(x => x.khachHang).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();



                var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
                var banProjection = Builders<Ban>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenBan);
                var bans = await _collectionBan.Find(banFilter)
                    .Project<Ban>(banProjection)
                    .ToListAsync();


                var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
                var loaiDonProjection = Builders<LoaiDon>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenLoaiDon);
                var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
                    .Project<LoaiDon>(loaiDonProjection)
                    .ToListAsync();


                var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
                var khachHangProjection = Builders<KhachHang>.Projection
                   .Include(x => x.Id)
                   .Include(x => x.tenKhachHang);
                var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
                  .Project<KhachHang>(khachHangProjection)
                  .ToListAsync();



                var banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);
                var loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);
                var monAnDict = new Dictionary<string, string>();
                var khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);
                var comBoDict = new Dictionary<string, string>();

                List<MonAn> monAns = new List<MonAn>();
                foreach (var don in dons)
                {
                    var monAnIds = dons.SelectMany(x => x.chiTietDonOrder)
                        .SelectMany(ct => ct.monAns)
                        .Select(ma => ma.monAn)
                        .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);

                    var monAnProjection = Builders<MonAn>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenMonAn)
                        .Include(x => x.giaTien);

                    var newMonAns = await _collectionMonAn.Find(monAnFilter)
                        .Project<MonAn>(monAnProjection)
                        .ToListAsync();

                    var uniqueMonAns = newMonAns.Where(x => !monAns.Any(y => y.Id == x.Id));
                    monAns.AddRange(uniqueMonAns);

                    var newDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
                    foreach (var item in newDict)
                    {
                        if (!monAnDict.ContainsKey(item.Key))
                        {
                            monAnDict.Add(item.Key, item.Value);
                        }
                    }
                }

                List<Combo> comBos = new List<Combo>();
                foreach (var don in dons)
                {
                    var comBoIds = dons.SelectMany(x => x.chiTietDonOrder)
                       .SelectMany(ct => ct.comBos)
                       .Select(ma => ma.comBo)
                       .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);

                    var comBoProjection = Builders<Combo>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenCombo)
                        .Include(x => x.giaTien);

                    var newComBos = await _collectionCombo.Find(comBoFilter)
                    .Project<Combo>(comBoProjection)
                    .ToListAsync();

                    var uniqueComBos = newComBos.Where(x => !comBos.Any(y => y.Id == x.Id));
                    comBos.AddRange(uniqueComBos);

                    var newComBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);
                    foreach (var item in newComBoDict)
                    {
                        if (!comBoDict.ContainsKey(item.Key))
                        {
                            comBoDict.Add(item.Key, item.Value);
                        }
                    }
                }


                var donOrderResponds = dons.Select(donOrder => new DonOrderRespond
                {
                    id = donOrder.Id,
                    tenDon = donOrder.tenDon,
                    loaiDon = new IdName
                    {
                        Id = donOrder.loaiDon,
                        Name = donOrder.loaiDon != null && loaiDonDict.ContainsKey(donOrder.loaiDon) ? loaiDonDict[donOrder.loaiDon] : null
                    },
                    ban = new IdName
                    {
                        Id = donOrder.ban,
                        Name = donOrder.ban != null && banDict.ContainsKey(donOrder.ban) ? banDict[donOrder.ban] : null
                    },
                    khachHang = new IdName
                    {
                        Id = donOrder.khachHang,
                        Name = donOrder.khachHang != null && khachHangDict.ContainsKey(donOrder.khachHang) ? khachHangDict[donOrder.khachHang] : null
                    },
                    chiTietDonOrder = donOrder.chiTietDonOrder.Select(ct => new ChiTietDonOrderRespond
                    {
                        monAns = ct.monAns.Select(ma => new DonMonAnRespond
                        {
                            monAn = new IdName
                            {
                                Id = ma.monAn,
                                Name = ma.monAn != null && monAnDict.ContainsKey(ma.monAn) ? monAnDict[ma.monAn] : null
                            },

                            monAn_trangThai = ma.monAn_trangThai,
                            soLuong = ma.soLuong,
                            giaTien = monAnDict.ContainsKey(ma.monAn) ? monAns.FirstOrDefault(m => m.Id == ma.monAn)?.giaTien : null,
                            moTa = ma.moTa,
                        }).ToList(),
                        comBos = ct.comBos.Select(ma => new DonComBoRespond
                        {
                            comBo = new IdName
                            {
                                Id = ma.comBo,
                                Name = ma.comBo != null && comBoDict.ContainsKey(ma.comBo) ? comBoDict[ma.comBo] : null
                            },
                            comBo_trangThai = ma.comBo_trangThai,
                            soLuong = ma.soLuong,
                            giaTien = comBoDict.ContainsKey(ma.comBo) ? comBos.FirstOrDefault(m => m.Id == ma.comBo)?.giaTien : null,
                            moTa = ma.moTa,
                        }).ToList(),
                        trangThai = ct.trangThai,
                    }).ToList(),
                    trangThai = donOrder.trangThai,
                    tongTien = donOrder.tongTien,
                    createdDate = donOrder.createdDate?.Date,
                    ngayTao = donOrder.createdDate,
                }).OrderBy(x => x.trangThai)
                    .ThenByDescending(x => x.ngayTao).ToList();

                return new RespondAPIPaging<List<DonOrderRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<DonOrderRespond>>
                    {
                        Data = donOrderResponds,
                        Paging = new PagingDetail(1, donOrderResponds.Count(), donOrderResponds.Count())
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<DonOrderRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<DonOrderRespond>> GetDonOrderById(string id)
    {
        try
        {
            var donOrder = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (donOrder == null)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn order với ID đã cung cấp."
                );
            }


            var banIds = new List<string> { donOrder.ban }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


            var loaiDonIds = new List<string> { donOrder.loaiDon }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();


            var khachHangIds = new List<string> { donOrder.khachHang }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();



            var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
            var banProjection = Builders<Ban>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenBan);
            var bans = await _collectionBan.Find(banFilter)
                .Project<Ban>(banProjection)
                .ToListAsync();


            var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
            var loaiDonProjection = Builders<LoaiDon>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenLoaiDon);
            var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
                .Project<LoaiDon>(loaiDonProjection)
                .ToListAsync();


            var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
            var khachHangProjection = Builders<KhachHang>.Projection
              .Include(x => x.Id)
              .Include(x => x.tenKhachHang);
            var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
             .Project<KhachHang>(khachHangProjection)
             .ToListAsync();



            var banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);
            var loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);
            var monAnDict = new Dictionary<string, string>();
            var khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);
            var comBoDict = new Dictionary<string, string>();

            List<Combo> comBos = new List<Combo>();
            List<MonAn> monAns = new List<MonAn>();
            foreach (var don in donOrder.chiTietDonOrder)
            {
                var monAnIds = donOrder.chiTietDonOrder.SelectMany(x => x.monAns)
                    .Select(ma => ma.monAn)
                    .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var comBoIds = donOrder.chiTietDonOrder.SelectMany(x => x.comBos)
                   .Select(ma => ma.comBo)
                   .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
                var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);

                var monAnProjection = Builders<MonAn>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenMonAn)
                    .Include(x => x.giaTien);
                var comBoProjection = Builders<Combo>.Projection
                   .Include(x => x.Id)
                   .Include(x => x.tenCombo)
                   .Include(x => x.giaTien);

                var newMonAns = await _collectionMonAn.Find(monAnFilter)
                    .Project<MonAn>(monAnProjection)
                    .ToListAsync();

                var newComBos = await _collectionCombo.Find(comBoFilter)
                    .Project<Combo>(comBoProjection)
                    .ToListAsync();

                var uniqueMonAns = newMonAns.Where(x => !monAns.Any(y => y.Id == x.Id));
                monAns.AddRange(uniqueMonAns);

                var uniqueComBos = newComBos.Where(x => !comBos.Any(y => y.Id == x.Id));
                comBos.AddRange(uniqueComBos);

                var newDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
                foreach (var item in newDict)
                {
                    if (!monAnDict.ContainsKey(item.Key))
                    {
                        monAnDict.Add(item.Key, item.Value);
                    }
                }

                var newComBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);
                foreach (var item in newComBoDict)
                {
                    if (!comBoDict.ContainsKey(item.Key))
                    {
                        comBoDict.Add(item.Key, item.Value);
                    }
                }
            }


            var donOrderRespond = new DonOrderRespond
            {
                id = donOrder.Id,
                tenDon = donOrder.tenDon,
                loaiDon = new IdName
                {
                    Id = donOrder.loaiDon,
                    Name = donOrder.loaiDon != null && loaiDonDict.ContainsKey(donOrder.loaiDon) ? loaiDonDict[donOrder.loaiDon] : null
                },
                ban = new IdName
                {
                    Id = donOrder.ban,
                    Name = donOrder.ban != null && banDict.ContainsKey(donOrder.ban) ? banDict[donOrder.ban] : null
                },
                khachHang = new IdName
                {
                    Id = donOrder.khachHang,
                    Name = donOrder.khachHang != null && khachHangDict.ContainsKey(donOrder.khachHang) ? khachHangDict[donOrder.khachHang] : null
                },
                chiTietDonOrder = donOrder.chiTietDonOrder.Select(ct => new ChiTietDonOrderRespond
                {
                    monAns = ct.monAns.Select(ma => new DonMonAnRespond
                    {
                        monAn = new IdName
                        {
                            Id = ma.monAn,
                            Name = ma.monAn != null && monAnDict.ContainsKey(ma.monAn) ? monAnDict[ma.monAn] : null
                        },

                        monAn_trangThai = ma.monAn_trangThai,
                        soLuong = ma.soLuong,
                        giaTien = monAnDict.ContainsKey(ma.monAn) ? monAns.FirstOrDefault(m => m.Id == ma.monAn)?.giaTien : null,
                        moTa = ma.moTa,
                    }).ToList(),
                    comBos = ct.comBos.Select(ma => new DonComBoRespond
                    {
                        comBo = new IdName
                        {
                            Id = ma.comBo,
                            Name = ma.comBo != null && comBoDict.ContainsKey(ma.comBo) ? comBoDict[ma.comBo] : null
                        },
                        comBo_trangThai = ma.comBo_trangThai,
                        soLuong = ma.soLuong,
                        giaTien = comBoDict.ContainsKey(ma.comBo) ? comBos.FirstOrDefault(m => m.Id == ma.comBo)?.giaTien : null,
                        moTa = ma.moTa,
                    }).ToList(),
                    trangThai = ct.trangThai,
                }).ToList(),
                trangThai = donOrder.trangThai,
                tongTien = donOrder.tongTien,
                createdDate = donOrder.createdDate?.Date,
            };

            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Succeeded,
                "Lấy đơn order thành công.",
                donOrderRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DonOrderRespond>> CreateDonOrder(RequestAddDonOrder request)
    {
        try
        {
            DonOrder newDonOrder = _mapper.Map<DonOrder>(request);

            newDonOrder.createdDate = DateTimeOffset.UtcNow;
            newDonOrder.updatedDate = DateTimeOffset.UtcNow;
            newDonOrder.isDelete = false;

            await _collection.InsertOneAsync(newDonOrder);



            var monAnDict = new Dictionary<string, string>();
            var loaiDonDict = new Dictionary<string, string>();
            var banDict = new Dictionary<string, string>();
            var khachHangDict = new Dictionary<string, string>();
            var comBoDict = new Dictionary<string, string>();

            var loaiDonIds = new List<string> { newDonOrder.loaiDon }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var banIds = new List<string> { newDonOrder.ban }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var khachHangIds = new List<string> { newDonOrder.khachHang }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
            var khachHangProjection = Builders<KhachHang>.Projection
            .Include(x => x.Id)
            .Include(x => x.tenKhachHang);
            var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
                .Project<KhachHang>(khachHangProjection)
               .ToListAsync();
            khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);

            var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
            var loaiDonProjection = Builders<LoaiDon>.Projection
              .Include(x => x.Id)
             .Include(x => x.tenLoaiDon);
            var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
             .Project<LoaiDon>(loaiDonProjection)
             .ToListAsync();
            loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);

            var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
            var banProjection = Builders<Ban>.Projection
             .Include(x => x.Id)
            .Include(x => x.tenBan);
            var bans = await _collectionBan.Find(banFilter)
            .Project<Ban>(banProjection)
            .ToListAsync();
            banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);

            List<Combo> comBos = new List<Combo>();
            List<MonAn> monAns = new List<MonAn>();

            var monAnIds = newDonOrder.chiTietDonOrder.SelectMany(x => x.monAns.Select(y => y.monAn)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var comBoIds = newDonOrder.chiTietDonOrder.SelectMany(x => x.comBos.Select(y => y.comBo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);
            var comBoProjection = Builders<Combo>.Projection
              .Include(x => x.Id)
             .Include(x => x.tenCombo)
             .Include(x => x.giaTien)
            .Include(x => x.moTa);
            comBos = await _collectionCombo.Find(comBoFilter)
              .Project<Combo>(comBoProjection)
              .ToListAsync();

            var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
            var monAnProjection = Builders<MonAn>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenMonAn)
                .Include(x => x.giaTien)
                .Include(x => x.moTa);
            monAns = await _collectionMonAn.Find(monAnFilter)
                .Project<MonAn>(monAnProjection)
                .ToListAsync();

            monAnDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
            comBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);


            var donOrderRespond = new DonOrderRespond
            {
                id = newDonOrder.Id,
                tenDon = newDonOrder.tenDon,
                loaiDon = new IdName
                {
                    Id = newDonOrder.loaiDon,
                    Name = loaiDonDict.ContainsKey(newDonOrder.loaiDon) ? loaiDonDict[newDonOrder.loaiDon] : null
                },
                ban = new IdName
                {
                    Id = newDonOrder.ban,
                    Name = banDict.ContainsKey(newDonOrder.ban) ? banDict[newDonOrder.ban] : null
                },
                khachHang = new IdName
                {
                    Id = newDonOrder.khachHang,
                    Name = khachHangDict.ContainsKey(newDonOrder.khachHang) ? khachHangDict[newDonOrder.khachHang] : null
                },
                chiTietDonOrder = newDonOrder.chiTietDonOrder.Select(x => new ChiTietDonOrderRespond
                {
                    monAns = x.monAns.Select(y => new DonMonAnRespond
                    {
                        monAn = new IdName
                        {
                            Id = y.monAn,
                            Name = monAnDict.ContainsKey(y.monAn) ? monAnDict[y.monAn] : null,
                        },
                        monAn_trangThai = y.monAn_trangThai,
                        soLuong = y.soLuong,
                        giaTien = monAnDict.ContainsKey(y.monAn) ? monAns.FirstOrDefault(m => m.Id == y.monAn)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    comBos = x.comBos.Select(y => new DonComBoRespond
                    {
                        comBo = new IdName
                        {
                            Id = y.comBo,
                            Name = comBoDict.ContainsKey(y.comBo) ? comBoDict[y.comBo] : null,
                        },
                        comBo_trangThai = y.comBo_trangThai,
                        soLuong = y.soLuong,
                        giaTien = comBoDict.ContainsKey(y.comBo) ? comBos.FirstOrDefault(m => m.Id == y.comBo)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    trangThai = x.trangThai
                }).ToList(),
                trangThai = newDonOrder.trangThai,
                tongTien = newDonOrder.tongTien
            };

            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Succeeded,
                "Tạo đơn order thành công.",
                donOrderRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo đơn order: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DonOrderRespond>> UpdateDonOrder(string id, RequestUpdateDonOrder request)
    {
        try
        {
            var filter = Builders<DonOrder>.Filter.Eq(x => x.Id, id);
            filter &= Builders<DonOrder>.Filter.Eq(x => x.isDelete, false);
            var donOrder = await _collection.Find(filter).FirstOrDefaultAsync();

            if (donOrder == null)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn order với ID đã cung cấp."
                );
            }

            _mapper.Map(request, donOrder);

            donOrder.updatedDate = DateTimeOffset.UtcNow;


            var updateResult = await _collection.ReplaceOneAsync(filter, donOrder);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.Error,
                    "Cập nhật đơn order không thành công."
                );
            }

            var monAnDict = new Dictionary<string, string>();
            var loaiDonDict = new Dictionary<string, string>();
            var banDict = new Dictionary<string, string>();
            var khachHangDict = new Dictionary<string, string>();
            var comBoDict = new Dictionary<string, string>();

            var loaiDonIds = new List<string> { donOrder.loaiDon }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var banIds = new List<string> { donOrder.ban }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var khachHangIds = new List<string> { donOrder.khachHang }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
            var khachHangProjection = Builders<KhachHang>.Projection
                .Include(x => x.Id)
               .Include(x => x.tenKhachHang);
            var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
               .Project<KhachHang>(khachHangProjection)
              .ToListAsync();
            khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);

            var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
            var loaiDonProjection = Builders<LoaiDon>.Projection
              .Include(x => x.Id)
             .Include(x => x.tenLoaiDon);
            var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
             .Project<LoaiDon>(loaiDonProjection)
             .ToListAsync();
            loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);

            var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
            var banProjection = Builders<Ban>.Projection
             .Include(x => x.Id)
            .Include(x => x.tenBan);
            var bans = await _collectionBan.Find(banFilter)
            .Project<Ban>(banProjection)
            .ToListAsync();
            banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);

            List<Combo> comBos = new List<Combo>();
            List<MonAn> monAns = new List<MonAn>();

            var monAnIds = donOrder.chiTietDonOrder.SelectMany(x => x.monAns.Select(y => y.monAn)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var comBoIds = donOrder.chiTietDonOrder.SelectMany(x => x.comBos.Select(y => y.comBo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);
            var comBoProjection = Builders<Combo>.Projection
             .Include(x => x.Id)
            .Include(x => x.tenCombo)
            .Include(x => x.giaTien)
            .Include(x => x.moTa);
            comBos = await _collectionCombo.Find(comBoFilter)
             .Project<Combo>(comBoProjection)
             .ToListAsync();

            var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
            var monAnProjection = Builders<MonAn>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenMonAn)
                .Include(x => x.giaTien)
                .Include(x => x.moTa);
            monAns = await _collectionMonAn.Find(monAnFilter)
                .Project<MonAn>(monAnProjection)
                .ToListAsync();

            monAnDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
            comBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);

            var donOrderRespond = new DonOrderRespond
            {
                id = donOrder.Id,
                tenDon = donOrder.tenDon,
                loaiDon = new IdName
                {
                    Id = donOrder.loaiDon,
                    Name = loaiDonDict.ContainsKey(donOrder.loaiDon) ? loaiDonDict[donOrder.loaiDon] : null
                },
                ban = new IdName
                {
                    Id = donOrder.ban,
                    Name = banDict.ContainsKey(donOrder.ban) ? banDict[donOrder.ban] : null
                },
                khachHang = new IdName
                {
                    Id = donOrder.khachHang,
                    Name = khachHangDict.ContainsKey(donOrder.khachHang) ? khachHangDict[donOrder.khachHang] : null
                },
                chiTietDonOrder = donOrder.chiTietDonOrder.Select(x => new ChiTietDonOrderRespond
                {
                    monAns = x.monAns.Select(y => new DonMonAnRespond
                    {
                        monAn = new IdName
                        {
                            Id = y.monAn,
                            Name = monAnDict.ContainsKey(y.monAn) ? monAnDict[y.monAn] : null,
                        },
                        monAn_trangThai = y.monAn_trangThai,
                        soLuong = y.soLuong,
                        giaTien = monAnDict.ContainsKey(y.monAn) ? monAns.FirstOrDefault(m => m.Id == y.monAn)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    comBos = x.comBos.Select(y => new DonComBoRespond
                    {
                        comBo = new IdName
                        {
                            Id = y.comBo,
                            Name = comBoDict.ContainsKey(y.comBo) ? comBoDict[y.comBo] : null,
                        },
                        comBo_trangThai = y.comBo_trangThai,
                        soLuong = y.soLuong,
                        giaTien = comBoDict.ContainsKey(y.comBo) ? comBos.FirstOrDefault(m => m.Id == y.comBo)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    trangThai = x.trangThai
                }).ToList(),
                trangThai = donOrder.trangThai,
                tongTien = donOrder.tongTien
            };
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Succeeded,
                "Cập nhật đơn order thành công.",
                donOrderRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật đơn order: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteDonOrder(string id)
    {
        try
        {
            var existingDonOrder = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingDonOrder == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn order để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa đơn order không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa đơn order thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa đơn order: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<DonOrderRespond>> UpdateStatusDonOrder(string id, RequestUpdateStatusDonOrder request)
    {
        try
        {
            var filter = Builders<DonOrder>.Filter.Eq(x => x.Id, id);
            filter &= Builders<DonOrder>.Filter.Eq(x => x.isDelete, false);
            var donOrder = await _collection.Find(filter).FirstOrDefaultAsync();

            if (donOrder == null)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy đơn order với ID đã cung cấp."
                );
            }

            if (request.trangThai == null)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.Error,
                    "Trạng thái đơn order không được để trống."
                );
            }

            donOrder.trangThai = request.trangThai;
            donOrder.updatedDate = DateTimeOffset.UtcNow;

            var updateResult = await _collection.ReplaceOneAsync(filter, donOrder);



            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<DonOrderRespond>(
                    ResultRespond.Error,
                    "Cập nhật trạng thái đơn order không thành công."
                );
            }

            var monAnDict = new Dictionary<string, string>();
            var loaiDonDict = new Dictionary<string, string>();
            var banDict = new Dictionary<string, string>();
            var khachHangDict = new Dictionary<string, string>();
            var comBoDict = new Dictionary<string, string>();

            var loaiDonIds = new List<string> { donOrder.loaiDon }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var banIds = new List<string> { donOrder.ban }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var khachHangIds = new List<string> { donOrder.khachHang }.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var khachHangFilter = Builders<KhachHang>.Filter.In(x => x.Id, khachHangIds);
            var khachHangProjection = Builders<KhachHang>.Projection
                .Include(x => x.Id)
               .Include(x => x.tenKhachHang);
            var khachHangs = await _collectionkhachHang.Find(khachHangFilter)
               .Project<KhachHang>(khachHangProjection)
              .ToListAsync();
            khachHangDict = khachHangs.ToDictionary(x => x.Id, x => x.tenKhachHang);

            var loaiDonFilter = Builders<LoaiDon>.Filter.In(x => x.Id, loaiDonIds);
            var loaiDonProjection = Builders<LoaiDon>.Projection
              .Include(x => x.Id)
             .Include(x => x.tenLoaiDon);
            var loaiDons = await _collectionLoaiDon.Find(loaiDonFilter)
             .Project<LoaiDon>(loaiDonProjection)
             .ToListAsync();
            loaiDonDict = loaiDons.ToDictionary(x => x.Id, x => x.tenLoaiDon);

            var banFilter = Builders<Ban>.Filter.In(x => x.Id, banIds);
            var banProjection = Builders<Ban>.Projection
             .Include(x => x.Id)
            .Include(x => x.tenBan);
            var bans = await _collectionBan.Find(banFilter)
            .Project<Ban>(banProjection)
            .ToListAsync();
            banDict = bans.ToDictionary(x => x.Id, x => x.tenBan);

            List<Combo> comBos = new List<Combo>();
            List<MonAn> monAns = new List<MonAn>();

            var monAnIds = donOrder.chiTietDonOrder.SelectMany(x => x.monAns.Select(y => y.monAn)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            var comBoIds = donOrder.chiTietDonOrder.SelectMany(x => x.comBos.Select(y => y.comBo)).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var comBoFilter = Builders<Combo>.Filter.In(x => x.Id, comBoIds);
            var comBoProjection = Builders<Combo>.Projection
             .Include(x => x.Id)
            .Include(x => x.tenCombo)
            .Include(x => x.giaTien)
            .Include(x => x.moTa);
            comBos = await _collectionCombo.Find(comBoFilter)
             .Project<Combo>(comBoProjection)
             .ToListAsync();

            var monAnFilter = Builders<MonAn>.Filter.In(x => x.Id, monAnIds);
            var monAnProjection = Builders<MonAn>.Projection
                .Include(x => x.Id)
                .Include(x => x.tenMonAn)
                .Include(x => x.giaTien)
                .Include(x => x.moTa);
            monAns = await _collectionMonAn.Find(monAnFilter)
                .Project<MonAn>(monAnProjection)
                .ToListAsync();

            monAnDict = monAns.ToDictionary(x => x.Id, x => x.tenMonAn);
            comBoDict = comBos.ToDictionary(x => x.Id, x => x.tenCombo);

            var donOrderRespond = new DonOrderRespond
            {
                id = donOrder.Id,
                tenDon = donOrder.tenDon,
                loaiDon = donOrder.loaiDon != null ? new IdName
                {
                    Id = donOrder.loaiDon,
                    Name = loaiDonDict.ContainsKey(donOrder.loaiDon) ? loaiDonDict[donOrder.loaiDon] : null
                } : new IdName
                {
                    Id = null,
                    Name = null,
                },
                ban = donOrder.ban != null ? new IdName
                {
                    Id = donOrder.ban,
                    Name = banDict.ContainsKey(donOrder.ban) ? banDict[donOrder.ban] : null
                } : new IdName
                {
                    Id = null,
                    Name = null,
                },
                khachHang = donOrder.khachHang != null ? new IdName
                {
                    Id = donOrder.khachHang,
                    Name = khachHangDict.ContainsKey(donOrder.khachHang) ? khachHangDict[donOrder.khachHang] : null
                } : new IdName
                {
                    Id = null,
                    Name = null,
                },
                chiTietDonOrder = donOrder.chiTietDonOrder.Select(x => new ChiTietDonOrderRespond
                {
                    monAns = x.monAns.Select(y => new DonMonAnRespond
                    {
                        monAn = new IdName
                        {
                            Id = y.monAn,
                            Name = monAnDict.ContainsKey(y.monAn) ? monAnDict[y.monAn] : null,
                        },
                        monAn_trangThai = y.monAn_trangThai,
                        soLuong = y.soLuong,
                        giaTien = monAnDict.ContainsKey(y.monAn) ? monAns.FirstOrDefault(m => m.Id == y.monAn)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    comBos = x.comBos.Select(y => new DonComBoRespond
                    {
                        comBo = new IdName
                        {
                            Id = y.comBo,
                            Name = comBoDict.ContainsKey(y.comBo) ? comBoDict[y.comBo] : null,
                        },
                        comBo_trangThai = y.comBo_trangThai,
                        soLuong = y.soLuong,
                        giaTien = comBoDict.ContainsKey(y.comBo) ? comBos.FirstOrDefault(m => m.Id == y.comBo)?.giaTien : null,
                        moTa = y.moTa,
                    }).ToList(),
                    trangThai = x.trangThai
                }).ToList(),
                trangThai = donOrder.trangThai,
                tongTien = donOrder.tongTien
            };
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Succeeded,
                "Cập nhật đơn order thành công.",
                donOrderRespond
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<DonOrderRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật đơn order: {ex.Message}"
            );
        }
    }

}