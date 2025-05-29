using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Requests.KhachHang;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;


[ApiController]
[Route("api/khach-hang")]


public class KhachHangController : ControllerBase
{
    private readonly IKhachHangRepository _khachHangRepository;

    public KhachHangController(IKhachHangRepository khachHangRepository)
    {
        _khachHangRepository = khachHangRepository;
    }


    [HttpGet("")]
    public async Task<IActionResult> GetAllKhachHangs([FromQuery] RequestSearchKhachHang request)
    {
        return Ok(await _khachHangRepository.GetAllKhachHangs(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetKhachHangById(string id)
    {
        return Ok(await _khachHangRepository.GetKhachHangById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateKhachHang(RequestAddKhachHang request)
    {
        return Ok(await _khachHangRepository.CreateKhachHang(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateKhachHang(string id, RequestUpdateKhachHang request)
    {
        return Ok(await _khachHangRepository.UpdateKhachHang(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKhachHang(string id)
    {
        return Ok(await _khachHangRepository.DeleteKhachHang(id));
    }
}
