using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiNguyenLieu;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/loai-nguyen-lieu")]
public class LoaiNguyenLieuController : ControllerBase
{
    private readonly ILoaiNguyenLieuRepository _repository;

    public LoaiNguyenLieuController(ILoaiNguyenLieuRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLoaiNguyenLieus([FromQuery] RequestSearchLoaiNguyenLieu request)
    {
        return Ok(await _repository.GetAllLoaiNguyenLieus(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoaiNguyenLieuById(string id)
    {
        return Ok(await _repository.GetLoaiNguyenLieuById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLoaiNguyenLieu(RequestAddLoaiNguyenLieu request)
    {
        return Ok(await _repository.CreateLoaiNguyenLieu(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoaiNguyenLieu(string id, RequestUpdateLoaiNguyenLieu request)
    {
        return Ok(await _repository.UpdateLoaiNguyenLieu(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoaiNguyenLieu(string id)
    {
        return Ok(await _repository.DeleteLoaiNguyenLieu(id));
    }
}