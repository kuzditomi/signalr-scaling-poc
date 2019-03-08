using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalrScalingPoc.PushService
{
    public class MyHub : Hub
    {
        public static string Name = Guid.NewGuid().ToString();

        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong");
        }

        public async Task SendMessage(string message)
        {
            var time = DateTime.Now.Ticks;
            await Clients.All.SendAsync("ReceiveMessage", message, time, Name);
        }
    }
}
