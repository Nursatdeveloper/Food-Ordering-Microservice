namespace Order.Service
{
    public class Dtos
    {
        public record EventDto(string Event);
        public record ChangeOrderStatusDto(int Id, string OrderStatus);

        public record OrderPublishedDto(string CustomerName, string Telephone, string DeliveryAddress, DateTime Date, string RestaurantName, string RestaurantAddress, string FoodCategory, string FoodName);
    }
}
