using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiMonAn;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/loai-mon-an")]
public class LoaiMonAnController : ControllerBase
{
    private readonly ILoaiMonAnRepository _repository;

    public LoaiMonAnController(ILoaiMonAnRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLoaiMonAns([FromQuery] RequestSearchLoaiMonAn request)
    {
        return Ok(await _repository.GetAllLoaiMonAns(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoaiMonAnById(string id)
    {
        return Ok(await _repository.GetLoaiMonAnById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateLoaiMonAn(RequestAddLoaiMonAn request)
    {
        return Ok(await _repository.CreateLoaiMonAn(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoaiMonAn(string id, RequestUpdateLoaiMonAn request)
    {
        return Ok(await _repository.UpdateLoaiMonAn(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoaiMonAn(string id)
    {
        return Ok(await _repository.DeleteLoaiMonAn(id));
    }
}