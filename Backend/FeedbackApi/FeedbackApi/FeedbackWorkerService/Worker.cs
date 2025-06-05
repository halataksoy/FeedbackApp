using System.Text;
using System.Text.Json;
using FeedbackApi.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMqSettings _rabbitSettings;
    private readonly MongoDbSettings _mongoSettings;
    private IModel _channel;
    private IConnection _connection;
    private IMongoCollection<FeedbackDto> _collection;

    public Worker(ILogger<Worker> logger, IConfiguration config)
    {
        _logger = logger;

        _rabbitSettings = config.GetSection("RabbitMQ").Get<RabbitMqSettings>();
        _mongoSettings = config.GetSection("MongoDB").Get<MongoDbSettings>();

        _logger.LogInformation("RabbitMQ ayarlarý: Host={Host}, Queue={Queue}", _rabbitSettings.HostName, _rabbitSettings.QueueName);
        _logger.LogInformation("MongoDB ayarlarý: ConnectionString={ConnStr}, Database={Db}, Collection={Col}",
            _mongoSettings.ConnectionString, _mongoSettings.DatabaseName, _mongoSettings.CollectionName);
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitSettings.HostName,
            UserName = _rabbitSettings.UserName,
            Password = _rabbitSettings.Password,
            Port = _rabbitSettings.Port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _rabbitSettings.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _logger.LogInformation("RabbitMQ kuyruðu hazýr: {Queue}", _rabbitSettings.QueueName);

        var client = new MongoClient(_mongoSettings.ConnectionString);
        var database = client.GetDatabase(_mongoSettings.DatabaseName);
        _collection = database.GetCollection<FeedbackDto>(_mongoSettings.CollectionName);
        _logger.LogInformation("MongoDB koleksiyonu hazýr: {Collection}", _mongoSettings.CollectionName);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            _logger.LogInformation("RabbitMQ'dan mesaj alýndý: {Json}", json);

            try
            {
                var feedback = JsonSerializer.Deserialize<FeedbackDto>(json);
                if (feedback != null)
                {
                    await _collection.InsertOneAsync(feedback);
                    _logger.LogInformation("Geri bildirim MongoDB'ye kaydedildi: {Email}", feedback.Email);
                }
                else
                {
                    _logger.LogWarning("Deserialization sonucu null geldi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deserialization veya MongoDB kaydý sýrasýnda hata oluþtu.");
            }
        };

        _channel.BasicConsume(queue: _rabbitSettings.QueueName, autoAck: true, consumer: consumer);

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker servis durduruluyor...");
            _channel.Close();
            _connection.Close();
        });

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}