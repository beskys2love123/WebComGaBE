using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace repo_nha_hang_com_ga_BE.Models.SignalR;

public class OrderHub : Hub
{
    public async Task SendOrder(string message)
    {
        await Clients.All.SendAsync("ReceiveOrder", message);
    }
}
