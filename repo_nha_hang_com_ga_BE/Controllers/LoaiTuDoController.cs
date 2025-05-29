using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.LoaiTuDo;
using repo_nha_hang_com_ga_BE.Models.Responds.LoaiTuDo;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/loai-tu-do")]
public class LoaiTuDoController : ControllerBase
{
    private readonly ILoaiTuDoRepository _repository;

    public LoaiTuDoController(ILoaiTuDoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllLoaiTuDos([FromQuery] RequestSearchLoaiTuDo request)
    {
        return Ok(await _repository.GetAllLoaiTuDos(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLoaiTuDoById(string id)
    {
        return Ok(await _repository.GetLoaiTuDoById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateLoaiTuDo(RequestAddLoaiTuDo request)
    {
        return Ok(await _repository.CreateLoaiTuDo(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLoaiTuDo(string id, RequestUpdateLoaiTuDo request)
    {
        return Ok(await _repository.UpdateLoaiTuDo(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLoaiTuDo(string id)
    {
        return Ok(await _repository.DeleteLoaiTuDo(id));
    }
}