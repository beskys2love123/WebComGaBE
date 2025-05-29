using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.BaoCaoThongKe;
using repo_nha_hang_com_ga_BE.Models.Requests.HoaDonThanhToan;
using repo_nha_hang_com_ga_BE.Models.Responds.HoaDonThanhToan;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/hoa-don-thanh-toan")]
public class HoaDonThanhToanController : ControllerBase
{
    private readonly IHoaDonThanhToanRepository _repository;

    public HoaDonThanhToanController(IHoaDonThanhToanRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllHoaDonThanhToans([FromQuery] RequestSearchHoaDonThanhToan request)
    {
        return Ok(await _repository.GetAllHoaDonThanhToan(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHoaDonThanhToanById(string id)
    {
        return Ok(await _repository.GetHoaDonThanhToanById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateHoaDonThanhToan(RequestAddHoaDonThanhToan request)
    {
        return Ok(await _repository.CreateHoaDonThanhToan(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHoaDonThanhToan(string id, RequestUpdateHoaDonThanhToan request)
    {
        return Ok(await _repository.UpdateHoaDonThanhToan(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHoaDonThanhToan(string id)
    {
        return Ok(await _repository.DeleteHoaDonThanhToan(id));
    }

    [HttpGet("doanh-thu")]
    public async Task<IActionResult> GetDoanhThu([FromQuery] RequestSearchThoiGian request)
    {
        return Ok(await _repository.GetDoanhThu(request));
    }

    [HttpGet("best-selling-mon-an")]
    public async Task<IActionResult> GetBestSellingMonAn([FromQuery] RequestSearchThoiGian request)
    {
        return Ok(await _repository.GetBestSellingMonAn(request));
    }

    [HttpGet("mat-do-khach-hang")]
    public async Task<IActionResult> GetMatDoKhachHang([FromQuery] RequestSearchThoiGian request)
    {
        return Ok(await _repository.GetMatDoKhachHang(request));
    }
}