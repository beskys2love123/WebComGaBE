using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Repositories;
using repo_nha_hang_com_ga_BE.Models.Requests.CaLamViec;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/ca-lam-viec")]

public class CaLamViecController : ControllerBase
{
    private readonly ICaLamViecRepository _repository;

    public CaLamViecController(ICaLamViecRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllCaLamViec([FromQuery] RequestSearchCaLamViec request)
    {
        return Ok(await _repository.GetAllCaLamViec(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaLamViecById(string id)
    {
        return Ok(await _repository.GetCaLamViecById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCaLamViec(RequestAddCaLamViec request)
    {
        return Ok(await _repository.CreateCaLamViec(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCaLamViec(string id, RequestUpdateCaLamViec request)
    {
        return Ok(await _repository.UpdateCaLamViec(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCaLamViec(string id)
    {
        return Ok(await _repository.DeleteCaLamViec(id));
    }
}
