using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Image.Grpc.Service.Models
{
    public class FoodImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Restaurant { get; set; }
        public string Food { get; set; }
        public byte[] Image { get; set; }
    }
}
