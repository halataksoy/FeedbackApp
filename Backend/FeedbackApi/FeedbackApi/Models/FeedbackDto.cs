using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FeedbackApi.Models
{
    public class FeedbackDto
    {
        [BsonId] //mongodb idsi
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
