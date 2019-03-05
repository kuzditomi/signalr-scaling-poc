using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalrScalingPoc.PushService
{
    public class MyHub : Hub
    {
        private static string Name = Guid.NewGuid().ToString();
        
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong");
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, Name);
        }
    }
}
