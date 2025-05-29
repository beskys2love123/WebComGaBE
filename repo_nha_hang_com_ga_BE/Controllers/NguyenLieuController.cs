using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.NguyenLieu;
using repo_nha_hang_com_ga_BE.Models.Responds.NguyenLieu;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/nguyen-lieu")]
public class NguyenLieuController : ControllerBase
{
    private readonly INguyenLieuRepository _repository;

    public NguyenLieuController(INguyenLieuRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllNguyenLieus([FromQuery] RequestSearchNguyenLieu request)
    {
        return Ok(await _repository.GetAllNguyenLieus(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNguyenLieuById(string id)
    {
        return Ok(await _repository.GetNguyenLieuById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateNguyenLieu(RequestAddNguyenLieu request)
    {
        return Ok(await _repository.CreateNguyenLieu(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNguyenLieu(string id, RequestUpdateNguyenLieu request)
    {
        return Ok(await _repository.UpdateNguyenLieu(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNguyenLieu(string id)
    {
        return Ok(await _repository.DeleteNguyenLieu(id));
    }
    [HttpPost("/add-list")]
    public async Task<IActionResult> CreateNguyenLieuList(RequestAddListNguyenLieu requests)
    {
        return Ok(await _repository.CreateListNguyenLieu(requests));
    }
}