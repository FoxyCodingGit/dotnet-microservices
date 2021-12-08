using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private IModel _channel;

        public MessageBusClient(IConfiguration configuration) // Remember that we can access "IConfiguration" without extra setup
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port =  int.Parse(_configuration["RabbitMQPort"])};

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout); // here we define what exchange type happens and that the exchange is called "trigger"

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {e}");
            }
        }
        
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection open, sending message...");
                SendMessage(message);
            }
            else
            {
                 Console.WriteLine("--> RabbitMQ connection closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body); // routing key is not for the fanout exchange type so can just be empty.
            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) // this happens when the connection shutdown event trgigers, we can have logic apply here if we want
        {
            Console.WriteLine("--> RabbitMq connection shutdown");
        }
    }
}