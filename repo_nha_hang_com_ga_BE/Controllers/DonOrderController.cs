using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using repo_nha_hang_com_ga_BE.Models.Requests.DonOrder;
using repo_nha_hang_com_ga_BE.Models.SignalR;
using repo_nha_hang_com_ga_BE.Repository;

namespace repo_nha_hang_com_ga_BE.Controllers;

[ApiController]
[Route("api/don-order")]

public class DonOrderController : ControllerBase
{
    private readonly IDonOrderRepository _repository;
    private readonly IHubContext<OrderHub> _hubContext;

    public DonOrderController(IDonOrderRepository repository, IHubContext<OrderHub> hubContext)
    {
        _repository = repository;
        _hubContext = hubContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllDonOrder([FromQuery] RequestSearchDonOrder request)
    {
        return Ok(await _repository.GetAllDonOrder(request));
    }

    [HttpPost("get-All")]
    public async Task<IActionResult> GetAllDonOrders([FromBody] RequestSearchDonOrder request)
    {
        return Ok(await _repository.GetAllDonOrder(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDonOrderById(string id)
    {
        return Ok(await _repository.GetDonOrderById(id));
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateDonOrder(RequestAddDonOrder request)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOrder", $"{request.tenDon}");

        return Ok(await _repository.CreateDonOrder(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonOrder(string id, RequestUpdateDonOrder request)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveOrder", $"{request.tenDon}");
        return Ok(await _repository.UpdateDonOrder(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonOrder(string id)
    {
        return Ok(await _repository.DeleteDonOrder(id));
    }

    [HttpPut("update-status/{id}")]
    public async Task<IActionResult> UpdateStatusDonOrder(string id, RequestUpdateStatusDonOrder request)
    {
        await _hubContext.Clients.All.SendAsync("ChangeStatusOrder", new
        {
            Id = id,
            Status = request.trangThai,
        });
        return Ok(await _repository.UpdateStatusDonOrder(id, request));
    }
}
