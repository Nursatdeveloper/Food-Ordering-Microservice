using Catalog.Service.PublishItems;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Catalog.Service.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_Connection_ShutDown;
                Console.WriteLine("Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
            }
        }
        public void RabbitMQ_Connection_ShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        private void SendMessage(string message)
        {
            if(_connection.IsOpen)
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(
                    exchange: "trigger",
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                Console.WriteLine($"--> Message sent: {message}");
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, could not send a message!");
            }

        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus Disposed");
            if (_channel!.IsOpen)
            {
                _channel.Close();
                _connection!.Close();
            }
        }

        public void PublishOrder(PublishOrder publishOrder)
        {
            var message = JsonSerializer.Serialize(publishOrder);
            SendMessage(message);
        }
    }
}
