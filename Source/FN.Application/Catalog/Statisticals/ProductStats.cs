using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FN.Application.Catalog.Statisticals
{
    public class ProductStats
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("productId")]
        public int ProductId { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        [BsonElement("viewCount")]
        public int ViewCount { get; set; } = 1;
    }
}
