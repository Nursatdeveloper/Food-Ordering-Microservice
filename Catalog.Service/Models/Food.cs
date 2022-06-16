namespace Catalog.Service.Models
{
    public class Food
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int CategoryId { get; set; }
        public string FoodName { get; set; }
        public string Ingredients { get; set; }
        public int Price { get; set; }
        public int PreparationTime { get; set; }
        
    }
}