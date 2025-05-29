namespace repo_nha_hang_com_ga_BE.Models.Requests;

public class RequestAddPhanQuyen
{
    public string? tenPhanQuyen { get; set; }
    public string? moTa { get; set; }

    public List<string>? danhSachMenu { get; set; }

}
