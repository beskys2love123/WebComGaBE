﻿using repo_nha_hang_com_ga_BE.Models.Common.Models;

namespace repo_nha_hang_com_ga_BE.Models.Requests.MonAn;

public class RequestAddMonAn
{
    public string? tenMonAn { get; set; }

    public string? loaiMonAn { get; set; }

    public string? congThuc { get; set; }

    public string? giamGia { get; set; }

    public string? moTa { get; set; }

    public string? hinhAnh { get; set; }

    public int? giaTien { get; set; }
}