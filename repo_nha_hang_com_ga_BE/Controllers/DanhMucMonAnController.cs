using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Models.Responds.DanhMucMonAn;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;

namespace repo_nha_hang_com_ga_BE.Controllers;

[Authorize]
[ApiController]
[Route("api/danh-muc-mon-an")]
public class DanhMucMonAnController : ControllerBase
{
    private readonly IDanhMucMonAnRepository _repository;

    public DanhMucMonAnController(IDanhMucMonAnRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDanhMucMonAns([FromQuery] RequestSearchDanhMucMonAn request)
    {
        return Ok(await _repository.GetAllDanhMucMonAns(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDanhMucMonAnById(string id)
    {
        return Ok(await _repository.GetDanhMucMonAnById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDanhMucMonAn(RequestAddDanhMucMonAn request)
    {
        return Ok(await _repository.CreateDanhMucMonAn(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDanhMucMonAn(string id, RequestUpdateDanhMucMonAn request)
    {
        return Ok(await _repository.UpdateDanhMucMonAn(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDanhMucMonAn(string id)
    {
        return Ok(await _repository.DeleteDanhMucMonAn(id));
    }
}