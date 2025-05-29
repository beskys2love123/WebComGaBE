using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucNguyenLieu;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/danh-muc-nguyen-lieu")]
public class DanhMucNguyenLieuController : ControllerBase
{
    private readonly IDanhMucNguyenLieuRepository _repository;

    public DanhMucNguyenLieuController(IDanhMucNguyenLieuRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDanhMucNguyenLieus([FromQuery] RequestSearchDanhMucNguyenLieu request)
    {
        return Ok(await _repository.GetAllDanhMucNguyenLieus(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDanhMucNguyenLieuById(string id)
    {
        return Ok(await _repository.GetDanhMucNguyenLieuById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDanhMucNguyenLieu(RequestAddDanhMucNguyenLieu request)
    {
        return Ok(await _repository.CreateDanhMucNguyenLieu(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDanhMucNguyenLieu(string id, RequestUpdateDanhMucNguyenLieu request)
    {
        return Ok(await _repository.UpdateDanhMucNguyenLieu(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDanhMucNguyenLieu(string id)
    {
        return Ok(await _repository.DeleteDanhMucNguyenLieu(id));
    }
}