using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Registration.Service.PublishItems;

namespace Registration.Service.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            var factory = new ConnectionFactory()
            {
                HostName = _env.IsDevelopment() ? 
                    _configuration["RabbitMQHost"] : 
                    Environment.GetEnvironmentVariable("RabbitMQHost"),
                Port = _env.IsDevelopment() ? 
                    int.Parse(_configuration["RabbitMQPort"]) : 
                    int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort")!)
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown!;
                Console.WriteLine("--> Connected to MessageBus");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection Shutdown");
        }

        private void SendMessage(string message)
        {
            if(_connection!.IsOpen)
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(
                    exchange: "trigger",
                    routingKey: "",
                    basicProperties: null,
                    body: body
                );
                Console.WriteLine($"==> Message sent: {message}");
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, could not send a message!");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Message Bus Disposed");
            if(_channel!.IsOpen)
            {
                _channel.Close();
                _connection!.Close();
            }
        }



        public void PublishFood(PublishFood publishFood)
        {
            var message = JsonSerializer.Serialize(publishFood);
            SendMessage(message);
        }

        public void PublishFoodCategory(PublishFoodCategory publishFoodCategory)
        {
            var message = JsonSerializer.Serialize(publishFoodCategory);
            SendMessage(message);
        }

        public void PublishRestaurant(PublishRestaurant publishRestaurant)
        {
            var message = JsonSerializer.Serialize(publishRestaurant);
            SendMessage(message);
        }

        public void PublishRestaurantAddress(PublishRestaurantAddress publishRestaurantAddress)
        {
            var message = JsonSerializer.Serialize(publishRestaurantAddress);
            SendMessage(message);
        }

        public void PublishOrderStreamingConnection(PublishOrderStreamingConnection publishOrderStreamingConnection)
        {
            var message = JsonSerializer.Serialize(publishOrderStreamingConnection);
            SendMessage(message);
        }
    }
}