using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandService.EventProcesing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService // runs in the background, a long running service, listening for events on the message bus
    {
        private readonly IConfiguration _configuration;
        private IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitialisingRabbitMQ();
        }

        private void InitialisingRabbitMQ() // creates the connection
        {
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port =  int.Parse(_configuration["RabbitMQPort"]) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the Message bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) // a long running task thats waiting and listening for events  
        {
            stoppingToken.ThrowIfCancellationRequested(); // request that this method stops if we pass in this token

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event recieved!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection shutdown");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}