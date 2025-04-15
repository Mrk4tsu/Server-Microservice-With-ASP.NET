using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FN.Application.Catalog.Product.Notifications
{
    public class UserNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; }
        public List<Notification> Notifications { get; set; }
    }
    public class Notification
    {
        [BsonElement]
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime Time { get; set; }
    }
}
