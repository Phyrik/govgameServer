using govgameWebApp.Hubs.Server_Only_Hub_Methods;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace govgameWebApp.Hubs
{
    public class TimeManagerHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
