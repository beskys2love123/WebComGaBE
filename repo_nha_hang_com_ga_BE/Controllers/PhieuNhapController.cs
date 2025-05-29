using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.PhieuNhap;
using repo_nha_hang_com_ga_BE.Models.Requests.BaoCaoThongKe;
using repo_nha_hang_com_ga_BE.Models.Responds.PhieuNhap;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/phieu-nhap")]
public class PhieuNhapController : ControllerBase
{
    private readonly IPhieuNhapRepository _repository;

    public PhieuNhapController(IPhieuNhapRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhieuNhaps([FromQuery] RequestSearchPhieuNhap request)
    {
        return Ok(await _repository.GetAllPhieuNhaps(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhieuNhapById(string id)
    {
        return Ok(await _repository.GetPhieuNhapById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePhieuNhap(RequestAddPhieuNhap request)
    {
        return Ok(await _repository.CreatePhieuNhap(request));
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhieuNhap(string id)
    {
        return Ok(await _repository.DeletePhieuNhap(id));
    }

    [HttpGet("khoan-chi")]
    public async Task<IActionResult> GetKhoanChi([FromQuery] RequestSearchThoiGian request)
    {
        return Ok(await _repository.GetKhoanChi(request));
    }
}