using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;
using repo_nha_hang_com_ga_BE.Models.Requests;

namespace repo_nha_hang_com_ga_BE.Controllers;


[Authorize]
[ApiController]
[Route("api/phan-quyen")]


public class PhanQuyenController : ControllerBase
{
    private readonly IPhanQuyenRepository _PhanQuyenRepository;

    public PhanQuyenController(IPhanQuyenRepository PhanQuyenRepository)
    {
        _PhanQuyenRepository = PhanQuyenRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhanQuyens([FromQuery] RequestSearchPhanQuyen request)
    {
        return Ok(await _PhanQuyenRepository.GetAllPhanQuyens(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhanQuyenById(string id)
    {
        return Ok(await _PhanQuyenRepository.GetPhanQuyenById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePhanQuyen(RequestAddPhanQuyen request)
    {
        return Ok(await _PhanQuyenRepository.CreatePhanQuyen(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePhanQuyen(string id, RequestUpdatePhanQuyen request)
    {
        return Ok(await _PhanQuyenRepository.UpdatePhanQuyen(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhanQuyen(string id)
    {
        return Ok(await _PhanQuyenRepository.DeletePhanQuyen(id));
    }
}
