using FeedbackApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FeedbackApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        public FeedbackController(IOptions<RabbitMqSettings> rabbitMqOptions)
        {
            _rabbitMqSettings = rabbitMqOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostFeedback([FromBody] FeedbackDto feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback.Name) ||
                string.IsNullOrWhiteSpace(feedback.Email) ||
                string.IsNullOrWhiteSpace(feedback.Message))
            {
                return BadRequest("Tüm alanlar zorunludur.");
            }

            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMqSettings.HostName,
                    UserName = _rabbitMqSettings.UserName,
                    Password = _rabbitMqSettings.Password,
                    Port = _rabbitMqSettings.Port
                };

                var connection = factory.CreateConnection();
                Console.WriteLine(connection.GetType().FullName);
                var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "feedbackQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = JsonSerializer.Serialize(feedback);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "feedbackQueue",
                                     basicProperties: null,
                                     body: body);

                return Ok(new { message = "Geri bildiriminiz alındı" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
    }
}

