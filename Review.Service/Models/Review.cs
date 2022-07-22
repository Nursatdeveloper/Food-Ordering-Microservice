using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reviews.Service.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ReviewItem {  get; set; }
        public int ReviewItemId { get; set; }
        public string? ReviewerName { get; set; }
        public bool IsReviewItemLiked { get; set; }
        public string? ReviewText { get; set; }
        public DateTime Date { get; set; }
    }
}
