using Order.Service.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Order.Service.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly IWebHostEnvironment _env;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _env = env;

            Start_RabbitMQ();
        }

        private void Start_RabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _env.IsDevelopment() ?
                    _configuration["RabbitMQHost"] :
                    Environment.GetEnvironmentVariable("RabbitMQHost"),
                Port = _env.IsDevelopment() ?
                    int.Parse(_configuration["RabbitMQPort"]) :
                    int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort")!)
            };
            Console.WriteLine($"--> C.S RabbitMQ: {factory.HostName} {factory.Port}");

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, exchange: "trigger", routingKey: "");
            Console.WriteLine("--> Listening on Message Bus...");

            _connection.ConnectionShutdown += RabbitMQ_Connection_Shutdown;
        }

        private void RabbitMQ_Connection_Shutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection shutdown!");
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
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("Order Service: Event Received!");
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(message);
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
