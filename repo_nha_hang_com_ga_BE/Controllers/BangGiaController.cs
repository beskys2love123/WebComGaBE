using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;
using repo_nha_hang_com_ga_BE.Models.Requests;
using repo_nha_hang_com_ga_BE.Models.Requests.BangGia;
using repo_nha_hang_com_ga_BE.Models.Responds.BangGia;


namespace repo_nha_hang_com_ga_BE.Controllers;


[Authorize]
[ApiController]
[Route("api/bang-gia")]


public class BangGiaController : ControllerBase
{
    private readonly IBangGiaRepository _BangGiaRepository;

    public BangGiaController(IBangGiaRepository BangGiaRepository)
    {
        _BangGiaRepository = BangGiaRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllBangGias([FromQuery] RequestSearchBangGia request)
    {
        return Ok(await _BangGiaRepository.GetAllBangGias(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBangGiaById(string id)
    {
        return Ok(await _BangGiaRepository.GetBangGiaById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateBangGia(RequestAddBangGia request)
    {
        return Ok(await _BangGiaRepository.CreateBangGia(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBangGia(string id, RequestUpdateBangGia request)
    {
        return Ok(await _BangGiaRepository.UpdateBangGia(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBangGia(string id)
    {
        return Ok(await _BangGiaRepository.DeleteBangGia(id));
    }
}
