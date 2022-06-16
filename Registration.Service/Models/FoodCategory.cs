namespace Registration.Service.Models
{
    public class FoodCategory
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string? CategoryName { get; set; }
    }
}