using Microsoft.AspNetCore.SignalR;

namespace SignalrScalingPoc.PushService
{
    public interface IPushClient
    {
        void Pong();
    }

    public class MyHub : Hub<IPushClient>
    {
        public void Ping()
        {
            Clients.Caller.Pong();
        }
    }
}
