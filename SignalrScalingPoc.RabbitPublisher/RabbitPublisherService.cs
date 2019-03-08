using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SignalrScalingPoc.RabbitPublisher
{
    public class RabbitPublisherService
    {
        private ConnectionFactory connectionFactory;
        private IModel channel;

        public RabbitPublisherService()
        {
            try
            {
                var rabbitaddress = "signalr-scaling-poc_rabbit_1";
                connectionFactory = new ConnectionFactory();

                connectionFactory.UserName = Environment.GetEnvironmentVariable("rabbit_user") ?? "rabbitmq";
                connectionFactory.Password = Environment.GetEnvironmentVariable("rabbit_password") ?? "rabbitmq";
                connectionFactory.VirtualHost = Environment.GetEnvironmentVariable("rabbit_vh") ?? "myvhost";
                connectionFactory.HostName = Environment.GetEnvironmentVariable("rabbit_host") ?? Dns.GetHostAddressesAsync(rabbitaddress).Result.FirstOrDefault().ToString();

                connectionFactory.AutomaticRecoveryEnabled = true;

                // connection that will recover automatically
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting rabbit consumer: {ex}");
            }
        }

        public void PublishMessages(int count)
        {
            Console.WriteLine($"Connecting to {connectionFactory.HostName} / {connectionFactory.VirtualHost} with credentials: {connectionFactory.UserName}:{connectionFactory.Password} ");

            var connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare("test.ex", ExchangeType.Fanout);
            channel.QueueDeclare("test.q", false, false, false, null);
            channel.QueueBind("test.q", "test.ex", "#", null);

            Console.WriteLine("Connected.");

            for (var i = 0; i < count; i++)
            {
                PublishMessage(i.ToString());
            }

            Console.WriteLine("Shutting down connection...");
            channel.Close();
            connection.Close();
            Console.WriteLine("Connection has shut down.");
        }

        private void PublishMessage(string message)
        {
            var messageBody = System.Text.Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("test.ex", routingKey: "#", body: messageBody);
        }
    }
}
