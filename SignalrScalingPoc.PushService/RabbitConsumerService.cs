using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SignalrScalingPoc.PushService
{
    public class RabbitConsumerService : IHostedService
    {
        private IModel channel;
        private IConnection connection;
        private readonly IHubContext<MyHub> hub;

        public RabbitConsumerService(IHubContext<MyHub> hub)
        {
            this.hub = hub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting rabbit consumer...");

            try
            {
                var rabbitaddress = "signalr-scaling-poc_rabbit_1";
                ConnectionFactory factory = new ConnectionFactory();

                factory.UserName = Environment.GetEnvironmentVariable("rabbit_user") ?? "rabbitmq";
                factory.Password = Environment.GetEnvironmentVariable("rabbit_password") ?? "rabbitmq";
                factory.VirtualHost = Environment.GetEnvironmentVariable("rabbit_vh") ?? "myvhost";
                factory.HostName = Environment.GetEnvironmentVariable("rabbit_host") ?? Dns.GetHostAddressesAsync(rabbitaddress).Result.FirstOrDefault().ToString();

                factory.AutomaticRecoveryEnabled = true;

                // connection that will recover automatically
                Console.WriteLine($"Connecting to {factory.HostName} / {factory.VirtualHost} with credentials: {factory.UserName}:{factory.Password} ");

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare("test.ex", ExchangeType.Fanout);
                channel.QueueDeclare("test.q", false, false, false, null);
                channel.QueueBind("test.q", "test.ex", "#", null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += OnMessageReceived;

                channel.BasicConsume("test.q", false, consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting rabbit consumer: {ex}");
            }
            Console.WriteLine("Rabbit consumer started.");

            return Task.CompletedTask;
        }

        private async void OnMessageReceived(object ch, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body;
            var message = System.Text.Encoding.UTF8.GetString(body); 

            var text = message.Split(",")[0];
            var date = message.Split(",")[1];
            Console.WriteLine($"Message arrived: {message} at {date}");

            await this.hub.Clients.All.SendAsync("ReceiveMessage", text, date, MyHub.Name);

            channel.BasicAck(eventArgs.DeliveryTag, false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Stopping rabbit consumer");

            if (channel != null && channel.IsOpen)
                channel.Close();

            if (connection != null && connection.IsOpen)
                connection.Close();

            return Task.CompletedTask;
        }
    }
}
