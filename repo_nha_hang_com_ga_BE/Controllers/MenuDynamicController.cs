using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Models.Common.Respond;
using repo_nha_hang_com_ga_BE.Models.Requests.MenuDynamic;
using repo_nha_hang_com_ga_BE.Models.Responds.MenuDynamic;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

// [Authorize]
[ApiController]
[Route("api/menu-dynamic")]
public class MenuDynamicController : ControllerBase
{
    private readonly IMenuDynamicRepository _repository;

    public MenuDynamicController(IMenuDynamicRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllMenuDynamics([FromQuery] RequestSearchMenuDynamic request)
    {
        return Ok(await _repository.GetAllMenuDynamics(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuDynamicById(string id)
    {
        return Ok(await _repository.GetMenuDynamicById(id));
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateMenuDynamic(RequestAddMenuDynamic request)
    {
        return Ok(await _repository.CreateMenuDynamic(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuDynamic(string id, RequestUpdateMenuDynamic request)
    {
        return Ok(await _repository.UpdateMenuDynamic(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuDynamic(string id)
    {
        return Ok(await _repository.DeleteMenuDynamic(id));
    }

    [HttpPost("phan-quyen")]
    public async Task<IActionResult> GetAllMenuDynamicsByPhanQuyen([FromBody] List<string> danhSachPhanQuyen)
    {
        return Ok(await _repository.GetAllMenuDynamicsByPhanQuyen(danhSachPhanQuyen));
    }
}