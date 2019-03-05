using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalrScalingPoc.PushService
{
    public class MyHub : Hub
    {
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong");
        }
    }
}
