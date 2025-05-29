using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Repositories;
using repo_nha_hang_com_ga_BE.Models.Requests.LichLamViec;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/lich-lam-viec")]

public class LichLamViecController : ControllerBase
{
    private readonly ILichLamViecRepository _repository;

    public LichLamViecController(ILichLamViecRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLichLamViec([FromQuery] RequestSearchLichLamViec request)
    {
        return Ok(await _repository.GetAllLichLamViecs(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLichLamViecById(string id)
    {
        return Ok(await _repository.GetLichLamViecById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLichLamViec(RequestAddLichLamViec request)
    {
        return Ok(await _repository.CreateLichLamViec(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLichLamViec(string id, RequestUpdateLichLamViec request)
    {
        return Ok(await _repository.UpdateLichLamViec(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLichLamViec(string id)
    {
        return Ok(await _repository.DeleteLichLamViec(id));
    }
}
