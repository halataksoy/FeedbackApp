﻿namespace FeedbackApi.Models
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string QueueName { get; set; } = null!;
        public int Port { get; set; } = 5672; // default
    }

}
