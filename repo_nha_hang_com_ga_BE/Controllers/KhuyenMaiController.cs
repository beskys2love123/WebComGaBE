using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.KhuyenMai;
using repo_nha_hang_com_ga_BE.Models.Responds.KhuyenMai;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/khuyen-mai")]
public class KhuyenMaiController : ControllerBase
{
    private readonly IKhuyenMaiRepository _repository;

    public KhuyenMaiController(IKhuyenMaiRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllKhuyenMais([FromQuery] RequestSearchKhuyenMai request)
    {
        return Ok(await _repository.GetAllKhuyenMais(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetKhuyenMaiById(string id)
    {
        return Ok(await _repository.GetKhuyenMaiById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateKhuyenMai(RequestAddKhuyenMai request)
    {
        return Ok(await _repository.CreateKhuyenMai(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateKhuyenMai(string id, RequestUpdateKhuyenMai request)
    {
        return Ok(await _repository.UpdateKhuyenMai(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKhuyenMai(string id)
    {
        return Ok(await _repository.DeleteKhuyenMai(id));
    }
}