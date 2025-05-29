
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
using repo_nha_hang_com_ga_BE.Models.Requests.LichLamViec;
using repo_nha_hang_com_ga_BE.Models.Responds.LichLamViecRespond;


namespace repo_nha_hang_com_ga_BE.Repository.Imp;

public class LichLamViecRepository : ILichLamViecRepository
{
    private readonly IMongoCollection<LichLamViec> _collection;
    private readonly IMongoCollection<CaLamViec> _collectionCaLamViec;
    private readonly IMongoCollection<NhanVien> _collectionNhanVien;
    private readonly IMapper _mapper;


    public LichLamViecRepository(IOptions<MongoDbSettings> settings, IMapper mapper)
    {
        var mongoClientSettings = settings.Value;
        var client = new MongoClient(mongoClientSettings.Connection);
        var database = client.GetDatabase(mongoClientSettings.DatabaseName);
        _collection = database.GetCollection<LichLamViec>("LichLamViec");
        _collectionCaLamViec = database.GetCollection<CaLamViec>("CaLamViec");
        _collectionNhanVien = database.GetCollection<NhanVien>("NhanVien");
        _mapper = mapper;
    }

    public async Task<RespondAPIPaging<List<LichLamViecRespond>>> GetAllLichLamViecs(RequestSearchLichLamViec request)
    {
        try
        {
            var collection = _collection;

            var filter = Builders<LichLamViec>.Filter.Empty;
            filter &= Builders<LichLamViec>.Filter.Eq(x => x.isDelete, false);

            if (request.ngay.HasValue)
            {
                filter &= Builders<LichLamViec>.Filter.Eq(x => x.ngay, request.ngay);
            }

            var projection = Builders<LichLamViec>.Projection
                .Include(x => x.Id)
                .Include(x => x.ngay)
               .Include(x => x.chiTietLichLamViec)
               .Include(x => x.moTa);

            var findOptions = new FindOptions<LichLamViec, LichLamViec>
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
                var lichLamViecs = await cursor.ToListAsync();

                var caLamViecDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();

                List<CaLamViec> caLamViecs = new List<CaLamViec>();
                foreach (var item in lichLamViecs)
                {
                    var caLamViecIds = item.chiTietLichLamViec
                      .Select(ct => ct.caLamViec)
                      .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var caLamViecFilter = Builders<CaLamViec>.Filter.In(x => x.Id, caLamViecIds);

                    var caLamViecProjection = Builders<CaLamViec>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenCaLamViec)
                        .Include(x => x.khungThoiGian)
                        .Include(x => x.moTa);

                    var newCaLamViecs = await _collectionCaLamViec.Find(caLamViecFilter)
                        .Project<CaLamViec>(caLamViecProjection)
                        .ToListAsync();

                    var uniqueCaLamViecs = newCaLamViecs.Where(x => !caLamViecs.Any(y => y.Id == x.Id));
                    caLamViecs.AddRange(uniqueCaLamViecs);

                    var newDict = caLamViecs.ToDictionary(x => x.Id, x => x.tenCaLamViec);
                    foreach (var ca in newDict)
                    {
                        if (!caLamViecDict.ContainsKey(ca.Key))
                        {
                            caLamViecDict.Add(ca.Key, ca.Value);
                        }
                    }
                }

                List<NhanVien> nhanViens = new List<NhanVien>();
                foreach (var item in lichLamViecs)
                {
                    var nhanVienIds = item.chiTietLichLamViec
                       .SelectMany(ct => ct.nhanVienCa)
                       .Select(nv => nv.nhanVien)
                       .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                    var nhanVienProjection = Builders<NhanVien>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNhanVien);


                    var newNhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();

                    var uniqueNhanViens = newNhanViens.Where(x => !nhanViens.Any(y => y.Id == x.Id));
                    nhanViens.AddRange(uniqueNhanViens);

                    var newNhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                    foreach (var y in newNhanVienDict)
                    {
                        if (!nhanVienDict.ContainsKey(y.Key))
                        {
                            nhanVienDict.Add(y.Key, y.Value);
                        }
                    }
                }

                // Map dữ liệu
                var lichLamViecResponds = lichLamViecs.Select(lichLamViec => new LichLamViecRespond
                {
                    id = lichLamViec.Id,
                    ngay = lichLamViec.ngay,
                    chiTietLichLamViec = lichLamViec.chiTietLichLamViec.Select(ct => new ChiTietLichLamViecRespond
                    {
                        caLamViec = new IdName
                        {
                            Id = ct.caLamViec,
                            Name = ct.caLamViec != null && caLamViecDict.ContainsKey(ct.caLamViec) ? caLamViecDict[ct.caLamViec] : null
                        },
                        nhanVienCa = ct.nhanVienCa.Select(nv => new NhanVienCaRespond
                        {
                            nhanVien = new IdName
                            {
                                Id = nv.nhanVien,
                                Name = nv.nhanVien != null && nhanVienDict.ContainsKey(nv.nhanVien) ? nhanVienDict[nv.nhanVien] : null
                            },
                            moTa = nv.moTa,
                        }).ToList(),
                        moTa = ct.moTa,
                    }).ToList(),
                    moTa = lichLamViec.moTa,
                }).OrderByDescending(x => x.ngay).ToList();
                var pagingDetail = new PagingDetail(currentPage, request.PageSize, totalRecords);
                var pagingResponse = new PagingResponse<List<LichLamViecRespond>>
                {
                    Paging = pagingDetail,
                    Data = lichLamViecResponds
                };

                return new RespondAPIPaging<List<LichLamViecRespond>>(
                    ResultRespond.Succeeded,
                    data: pagingResponse
                );
            }
            else
            {
                var cursor = await collection.FindAsync(filter, findOptions);
                var lichLamViecs = await cursor.ToListAsync();

                var caLamViecDict = new Dictionary<string, string>();
                var nhanVienDict = new Dictionary<string, string>();

                List<CaLamViec> caLamViecs = new List<CaLamViec>();
                foreach (var item in lichLamViecs)
                {
                    var caLamViecIds = item.chiTietLichLamViec
                      .Select(ct => ct.caLamViec)
                      .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var caLamViecFilter = Builders<CaLamViec>.Filter.In(x => x.Id, caLamViecIds);

                    var caLamViecProjection = Builders<CaLamViec>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenCaLamViec)
                        .Include(x => x.khungThoiGian)
                        .Include(x => x.moTa);

                    var newCaLamViecs = await _collectionCaLamViec.Find(caLamViecFilter)
                        .Project<CaLamViec>(caLamViecProjection)
                        .ToListAsync();

                    var uniqueCaLamViecs = newCaLamViecs.Where(x => !caLamViecs.Any(y => y.Id == x.Id));
                    caLamViecs.AddRange(uniqueCaLamViecs);

                    var newDict = caLamViecs.ToDictionary(x => x.Id, x => x.tenCaLamViec);
                    foreach (var ca in newDict)
                    {
                        if (!caLamViecDict.ContainsKey(ca.Key))
                        {
                            caLamViecDict.Add(ca.Key, ca.Value);
                        }
                    }
                }

                List<NhanVien> nhanViens = new List<NhanVien>();
                foreach (var item in lichLamViecs)
                {
                    var nhanVienIds = item.chiTietLichLamViec
                       .SelectMany(ct => ct.nhanVienCa)
                       .Select(nv => nv.nhanVien)
                       .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                    var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                    var nhanVienProjection = Builders<NhanVien>.Projection
                        .Include(x => x.Id)
                        .Include(x => x.tenNhanVien);


                    var newNhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                    .Project<NhanVien>(nhanVienProjection)
                    .ToListAsync();

                    var uniqueNhanViens = newNhanViens.Where(x => !nhanViens.Any(y => y.Id == x.Id));
                    nhanViens.AddRange(uniqueNhanViens);

                    var newNhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                    foreach (var y in newNhanVienDict)
                    {
                        if (!nhanVienDict.ContainsKey(y.Key))
                        {
                            nhanVienDict.Add(y.Key, y.Value);
                        }
                    }
                }

                // Map dữ liệu
                var lichLamViecResponds = lichLamViecs.Select(lichLamViec => new LichLamViecRespond
                {
                    id = lichLamViec.Id,
                    ngay = lichLamViec.ngay,
                    chiTietLichLamViec = lichLamViec.chiTietLichLamViec.Select(ct => new ChiTietLichLamViecRespond
                    {
                        caLamViec = new IdName
                        {
                            Id = ct.caLamViec,
                            Name = ct.caLamViec != null && caLamViecDict.ContainsKey(ct.caLamViec) ? caLamViecDict[ct.caLamViec] : null
                        },
                        nhanVienCa = ct.nhanVienCa.Select(nv => new NhanVienCaRespond
                        {
                            nhanVien = new IdName
                            {
                                Id = nv.nhanVien,
                                Name = nv.nhanVien != null && nhanVienDict.ContainsKey(nv.nhanVien) ? nhanVienDict[nv.nhanVien] : null
                            },
                            moTa = nv.moTa,
                        }).ToList(),
                        moTa = ct.moTa,
                    }).ToList(),
                    moTa = lichLamViec.moTa,
                }).OrderByDescending(x => x.ngay).ToList();

                return new RespondAPIPaging<List<LichLamViecRespond>>(
                    ResultRespond.Succeeded,
                    data: new PagingResponse<List<LichLamViecRespond>>
                    {
                        Data = lichLamViecResponds,
                        Paging = new PagingDetail(1, lichLamViecResponds.Count(), lichLamViecResponds.Count())
                    }
                );
            }
        }
        catch (Exception ex)
        {
            return new RespondAPIPaging<List<LichLamViecRespond>>(
                ResultRespond.Error,
                message: ex.Message
            );
        }
    }

    public async Task<RespondAPI<LichLamViecRespond>> GetLichLamViecById(string id)
    {
        try
        {
            var lichLamViec = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();

            if (lichLamViec == null)
            {
                return new RespondAPI<LichLamViecRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy lịch làm việc với ID đã cung cấp."
                );
            }


            // Tạo dictionary để map nhanh
            var caLamViecDict = new Dictionary<string, string>();
            var nhanVienDict = new Dictionary<string, string>();

            List<CaLamViec> caLamViecs = new List<CaLamViec>();
            foreach (var item in lichLamViec.chiTietLichLamViec)
            {
                var caLamViecIds = new List<string> { item.caLamViec }
                  .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var caLamViecFilter = Builders<CaLamViec>.Filter.In(x => x.Id, caLamViecIds);

                var caLamViecProjection = Builders<CaLamViec>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenCaLamViec)
                    .Include(x => x.khungThoiGian)
                    .Include(x => x.moTa);

                var newCaLamViecs = await _collectionCaLamViec.Find(caLamViecFilter)
                    .Project<CaLamViec>(caLamViecProjection)
                    .ToListAsync();

                var uniqueCaLamViecs = newCaLamViecs.Where(x => !caLamViecs.Any(y => y.Id == x.Id));
                caLamViecs.AddRange(uniqueCaLamViecs);

                var newDict = caLamViecs.ToDictionary(x => x.Id, x => x.tenCaLamViec);
                foreach (var ca in newDict)
                {
                    if (!caLamViecDict.ContainsKey(ca.Key))
                    {
                        caLamViecDict.Add(ca.Key, ca.Value);
                    }
                }
            }

            List<NhanVien> nhanViens = new List<NhanVien>();
            foreach (var item in lichLamViec.chiTietLichLamViec)
            {
                var nhanVienIds = item.nhanVienCa
                   .Select(nv => nv.nhanVien)
                   .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);


                var newNhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                .Project<NhanVien>(nhanVienProjection)
                .ToListAsync();

                var uniqueNhanViens = newNhanViens.Where(x => !nhanViens.Any(y => y.Id == x.Id));
                nhanViens.AddRange(uniqueNhanViens);

                var newNhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                foreach (var y in newNhanVienDict)
                {
                    if (!nhanVienDict.ContainsKey(y.Key))
                    {
                        nhanVienDict.Add(y.Key, y.Value);
                    }
                }
            }

            // Map dữ liệu
            var lichLamViecResponds = new LichLamViecRespond
            {
                id = lichLamViec.Id,
                ngay = lichLamViec.ngay,
                chiTietLichLamViec = lichLamViec.chiTietLichLamViec.Select(ct => new ChiTietLichLamViecRespond
                {
                    caLamViec = new IdName
                    {
                        Id = ct.caLamViec,
                        Name = ct.caLamViec != null && caLamViecDict.ContainsKey(ct.caLamViec) ? caLamViecDict[ct.caLamViec] : null
                    },
                    nhanVienCa = ct.nhanVienCa.Select(nv => new NhanVienCaRespond
                    {
                        nhanVien = new IdName
                        {
                            Id = nv.nhanVien,
                            Name = nv.nhanVien != null && nhanVienDict.ContainsKey(nv.nhanVien) ? nhanVienDict[nv.nhanVien] : null
                        },
                        moTa = nv.moTa,
                    }).ToList(),
                    moTa = ct.moTa,
                }).ToList(),
                moTa = lichLamViec.moTa,
            };

            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Succeeded,
                "Lấy lịch làm việc thành công.",
                lichLamViecResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LichLamViecRespond>> CreateLichLamViec(RequestAddLichLamViec request)
    {
        try
        {
            LichLamViec newLichLamViec = _mapper.Map<LichLamViec>(request);

            newLichLamViec.createdDate = DateTimeOffset.UtcNow;
            newLichLamViec.updatedDate = DateTimeOffset.UtcNow;
            newLichLamViec.isDelete = false;
            // Thiết lập createdUser và updatedUser nếu có thông tin người dùng
            // newDanhMucMonAn.createdUser = currentUser.Id;
            // newDanhMucNguyenLieu.updatedUser = currentUser.Id;

            await _collection.InsertOneAsync(newLichLamViec);



            // Tạo dictionary để map nhanh
            var caLamViecDict = new Dictionary<string, string>();
            var nhanVienDict = new Dictionary<string, string>();

            List<CaLamViec> caLamViecs = new List<CaLamViec>();
            foreach (var item in newLichLamViec.chiTietLichLamViec)
            {
                var caLamViecIds = new List<string> { item.caLamViec }
                  .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var caLamViecFilter = Builders<CaLamViec>.Filter.In(x => x.Id, caLamViecIds);

                var caLamViecProjection = Builders<CaLamViec>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenCaLamViec)
                    .Include(x => x.khungThoiGian)
                    .Include(x => x.moTa);

                var newCaLamViecs = await _collectionCaLamViec.Find(caLamViecFilter)
                    .Project<CaLamViec>(caLamViecProjection)
                    .ToListAsync();

                var uniqueCaLamViecs = newCaLamViecs.Where(x => !caLamViecs.Any(y => y.Id == x.Id));
                caLamViecs.AddRange(uniqueCaLamViecs);

                var newDict = caLamViecs.ToDictionary(x => x.Id, x => x.tenCaLamViec);
                foreach (var ca in newDict)
                {
                    if (!caLamViecDict.ContainsKey(ca.Key))
                    {
                        caLamViecDict.Add(ca.Key, ca.Value);
                    }
                }
            }

            List<NhanVien> nhanViens = new List<NhanVien>();
            foreach (var item in newLichLamViec.chiTietLichLamViec)
            {
                var nhanVienIds = item.nhanVienCa
                   .Select(nv => nv.nhanVien)
                   .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);


                var newNhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                .Project<NhanVien>(nhanVienProjection)
                .ToListAsync();

                var uniqueNhanViens = newNhanViens.Where(x => !nhanViens.Any(y => y.Id == x.Id));
                nhanViens.AddRange(uniqueNhanViens);

                var newNhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                foreach (var y in newNhanVienDict)
                {
                    if (!nhanVienDict.ContainsKey(y.Key))
                    {
                        nhanVienDict.Add(y.Key, y.Value);
                    }
                }
            }

            // Map dữ liệu
            var lichLamViecResponds = new LichLamViecRespond
            {
                id = newLichLamViec.Id,
                ngay = newLichLamViec.ngay,
                chiTietLichLamViec = newLichLamViec.chiTietLichLamViec.Select(ct => new ChiTietLichLamViecRespond
                {
                    caLamViec = new IdName
                    {
                        Id = ct.caLamViec,
                        Name = ct.caLamViec != null && caLamViecDict.ContainsKey(ct.caLamViec) ? caLamViecDict[ct.caLamViec] : null
                    },
                    nhanVienCa = ct.nhanVienCa.Select(nv => new NhanVienCaRespond
                    {
                        nhanVien = new IdName
                        {
                            Id = nv.nhanVien,
                            Name = nv.nhanVien != null && nhanVienDict.ContainsKey(nv.nhanVien) ? nhanVienDict[nv.nhanVien] : null
                        },
                        moTa = nv.moTa,
                    }).ToList(),
                    moTa = ct.moTa,
                }).ToList(),
                moTa = newLichLamViec.moTa,
            };

            // var loaiDon = await _collectionLoaiDon.Find(x => x.Id == newDonOrder.loaiDon).FirstOrDefaultAsync();
            // var donOrderRespond = _mapper.Map<DonOrderRespond>(newDonOrder);
            // donOrderRespond.loaiDon = new IdName
            // {
            //     Id = loaiDon.Id,
            //     Name = loaiDon.tenLoaiDon
            // };
            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Succeeded,
                "Tạo lịch làm việc thành công.",
                lichLamViecResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi tạo lịch làm việc: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<LichLamViecRespond>> UpdateLichLamViec(string id, RequestUpdateLichLamViec request)
    {
        try
        {
            var filter = Builders<LichLamViec>.Filter.Eq(x => x.Id, id);
            filter &= Builders<LichLamViec>.Filter.Eq(x => x.isDelete, false);
            var lichLamViec = await _collection.Find(filter).FirstOrDefaultAsync();

            if (lichLamViec == null)
            {
                return new RespondAPI<LichLamViecRespond>(
                    ResultRespond.NotFound,
                    "Không tìm thấy lịch làm việc với ID đã cung cấp."
                );
            }

            _mapper.Map(request, lichLamViec);

            lichLamViec.updatedDate = DateTimeOffset.UtcNow;

            // Cập nhật người dùng nếu có thông tin
            // danhMucNguyenLieu.updatedUser = currentUser.Id;

            var updateResult = await _collection.ReplaceOneAsync(filter, lichLamViec);

            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount == 0)
            {
                return new RespondAPI<LichLamViecRespond>(
                    ResultRespond.Error,
                    "Cập nhật lịch làm việc không thành công."
                );
            }

            // Tạo dictionary để map nhanh
            var caLamViecDict = new Dictionary<string, string>();
            var nhanVienDict = new Dictionary<string, string>();

            List<CaLamViec> caLamViecs = new List<CaLamViec>();
            foreach (var item in lichLamViec.chiTietLichLamViec)
            {
                var caLamViecIds = new List<string> { item.caLamViec }
                  .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var caLamViecFilter = Builders<CaLamViec>.Filter.In(x => x.Id, caLamViecIds);

                var caLamViecProjection = Builders<CaLamViec>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenCaLamViec)
                    .Include(x => x.khungThoiGian)
                    .Include(x => x.moTa);

                var newCaLamViecs = await _collectionCaLamViec.Find(caLamViecFilter)
                    .Project<CaLamViec>(caLamViecProjection)
                    .ToListAsync();

                var uniqueCaLamViecs = newCaLamViecs.Where(x => !caLamViecs.Any(y => y.Id == x.Id));
                caLamViecs.AddRange(uniqueCaLamViecs);

                var newDict = caLamViecs.ToDictionary(x => x.Id, x => x.tenCaLamViec);
                foreach (var ca in newDict)
                {
                    if (!caLamViecDict.ContainsKey(ca.Key))
                    {
                        caLamViecDict.Add(ca.Key, ca.Value);
                    }
                }
            }

            List<NhanVien> nhanViens = new List<NhanVien>();
            foreach (var item in lichLamViec.chiTietLichLamViec)
            {
                var nhanVienIds = item.nhanVienCa
                   .Select(nv => nv.nhanVien)
                   .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                var nhanVienFilter = Builders<NhanVien>.Filter.In(x => x.Id, nhanVienIds);

                var nhanVienProjection = Builders<NhanVien>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.tenNhanVien);


                var newNhanViens = await _collectionNhanVien.Find(nhanVienFilter)
                .Project<NhanVien>(nhanVienProjection)
                .ToListAsync();

                var uniqueNhanViens = newNhanViens.Where(x => !nhanViens.Any(y => y.Id == x.Id));
                nhanViens.AddRange(uniqueNhanViens);

                var newNhanVienDict = nhanViens.ToDictionary(x => x.Id, x => x.tenNhanVien);
                foreach (var y in newNhanVienDict)
                {
                    if (!nhanVienDict.ContainsKey(y.Key))
                    {
                        nhanVienDict.Add(y.Key, y.Value);
                    }
                }
            }

            // Map dữ liệu
            var lichLamViecResponds = new LichLamViecRespond
            {
                id = lichLamViec.Id,
                ngay = lichLamViec.ngay,
                chiTietLichLamViec = lichLamViec.chiTietLichLamViec.Select(ct => new ChiTietLichLamViecRespond
                {
                    caLamViec = new IdName
                    {
                        Id = ct.caLamViec,
                        Name = ct.caLamViec != null && caLamViecDict.ContainsKey(ct.caLamViec) ? caLamViecDict[ct.caLamViec] : null
                    },
                    nhanVienCa = ct.nhanVienCa.Select(nv => new NhanVienCaRespond
                    {
                        nhanVien = new IdName
                        {
                            Id = nv.nhanVien,
                            Name = nv.nhanVien != null && nhanVienDict.ContainsKey(nv.nhanVien) ? nhanVienDict[nv.nhanVien] : null
                        },
                        moTa = nv.moTa,
                    }).ToList(),
                    moTa = ct.moTa,
                }).ToList(),
                moTa = lichLamViec.moTa,
            };
            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Succeeded,
                "Cập nhật lịch làm việc thành công.",
                lichLamViecResponds
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<LichLamViecRespond>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi cập nhật lịch làm việc: {ex.Message}"
            );
        }
    }

    public async Task<RespondAPI<string>> DeleteLichLamViec(string id)
    {
        try
        {
            var existingLichLamViec = await _collection.Find(x => x.Id == id && x.isDelete == false).FirstOrDefaultAsync();
            if (existingLichLamViec == null)
            {
                return new RespondAPI<string>(
                    ResultRespond.NotFound,
                    "Không tìm thấy lịch làm việc để xóa."
                );
            }

            var deleteResult = await _collection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount == 0)
            {
                return new RespondAPI<string>(
                    ResultRespond.Error,
                    "Xóa lịch làm việc không thành công."
                );
            }

            return new RespondAPI<string>(
                ResultRespond.Succeeded,
                "Xóa lịch làm việc thành công.",
                id
            );
        }
        catch (Exception ex)
        {
            return new RespondAPI<string>(
                ResultRespond.Error,
                $"Đã xảy ra lỗi khi xóa lịch làm việc: {ex.Message}"
            );
        }
    }

}