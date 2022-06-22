using Catalog.Service.Models;

namespace Catalog.Service
{
    public record EventDto(string Event);
    public record PublishRestaurantDto(string Name);
    public record PublishRestaurantAddressDto(string RestaurantName, string Country, string City, string Street, string BuildingNumber);
    public record PublishFoodCategoryDto(string RestaurantName, string CategoryName);
    public record PublishFoodDto(string RestaurantName, string CategoryName, string FoodName, string Ingredients, int Price, int PreparationTime);
    public record RestaurantWithAddressDto(string RestaurantName, string city, List<Address> Addresses);
    public record RestaurantWithFoodCategoryDto(string RestaurantName, List<FoodCategoriesWithImageDto> FoodCategories);
    public record FoodsViewDto(string restaurantName, string categoryName, List<FoodWithImageDto> foods);
    public record FoodWithImageDto(int Id, string FoodName, string Ingredients, int Price, int PreparationTime, byte[] Image);
    public record RestaurantWithImageDto(int Id, string Restaurant, byte[] Image);
    public record FoodCategoriesWithImageDto(int Id, string Category, byte[] Image);

    public record CreateOrderDto(string CustomerName, string Telephone, string DeliveryAddress, int foodId);
}