using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.GiamGia;
using repo_nha_hang_com_ga_BE.Models.Responds.GiamGia;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/giam-gia")]
public class GiamGiaController : ControllerBase
{
    private readonly IGiamGiaRepository _repository;

    public GiamGiaController(IGiamGiaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllGiamGias([FromQuery] RequestSearchGiamGia request)
    {
        return Ok(await _repository.GetAllGiamGias(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGiamGiaById(string id)
    {
        return Ok(await _repository.GetGiamGiaById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateGiamGia(RequestAddGiamGia request)
    {
        return Ok(await _repository.CreateGiamGia(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGiamGia(string id, RequestUpdateGiamGia request)
    {
        return Ok(await _repository.UpdateGiamGia(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGiamGia(string id)
    {
        return Ok(await _repository.DeleteGiamGia(id));
    }
}