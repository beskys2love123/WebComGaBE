using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiKhuyenMai;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/loai-khuyen-mai")]
public class LoaiKhuyenMaiController : ControllerBase
{
    private readonly ILoaiKhuyenMaiRepository _repository;

    public LoaiKhuyenMaiController(ILoaiKhuyenMaiRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLoaiKhuyenMais([FromQuery] RequestSearchLoaiKhuyenMai request)
    {
        return Ok(await _repository.GetAllLoaiKhuyenMais(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoaiKhuyenMaiById(string id)
    {
        return Ok(await _repository.GetLoaiKhuyenMaiById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLoaiKhuyenMai(RequestAddLoaiKhuyenMai request)
    {
        return Ok(await _repository.CreateLoaiKhuyenMai(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoaiKhuyenMai(string id, RequestUpdateLoaiKhuyenMai request)
    {
        return Ok(await _repository.UpdateLoaiKhuyenMai(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoaiKhuyenMai(string id)
    {
        return Ok(await _repository.DeleteLoaiKhuyenMai(id));
    }
}