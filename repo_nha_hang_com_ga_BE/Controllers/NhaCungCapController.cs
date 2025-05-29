using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;
using repo_nha_hang_com_ga_BE.Models.Requests;

namespace repo_nha_hang_com_ga_BE.Controllers;


[Authorize]
[ApiController]
[Route("api/nha-cung-cap")]


public class NhaCungCapController : ControllerBase
{
    private readonly INhaCungCapRepository _NhaCungCapRepository;

    public NhaCungCapController(INhaCungCapRepository NhaCungCapRepository)
    {
        _NhaCungCapRepository = NhaCungCapRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllNhaCungCaps([FromQuery] RequestSearchNhaCungCap request)
    {
        return Ok(await _NhaCungCapRepository.GetAllNhaCungCaps(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNhaCungCapById(string id)
    {
        return Ok(await _NhaCungCapRepository.GetNhaCungCapById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateNhaCungCap(RequestAddNhaCungCap request)
    {
        return Ok(await _NhaCungCapRepository.CreateNhaCungCap(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNhaCungCap(string id, RequestUpdateNhaCungCap request)
    {
        return Ok(await _NhaCungCapRepository.UpdateNhaCungCap(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNhaCungCap(string id)
    {
        return Ok(await _NhaCungCapRepository.DeleteNhaCungCap(id));
    }
}
