namespace repo_nha_hang_com_ga_BE.Models.Requests;

public class RequestUpdatePhanQuyen
{
    public string? tenPhanQuyen { get; set; }
    public string? moTa { get; set; }
    public List<string>? danhSachMenu { get; set; }

}