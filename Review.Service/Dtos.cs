using Reviews.Service.Models;

namespace Reviews.Service
{
    public class Dtos
    {
        public record CreateReviewDto(
            string ReviewItem, 
            int ReviewItemId, 
            string ReviewerName, 
            bool IsReviewItemLiked,
            string ReviewText);

        public record ReviewsListDto(List<Review> reviews);

        public record ReviewStatisticsDto(int ReviewItemId, double PercentageOfLikes, string OverallReviews);
        public record ReviewStatisticsListDto(List<ReviewStatisticsDto> reviews);
    }
}
