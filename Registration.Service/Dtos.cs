using Registration.Service.Models;

namespace Registration.Service
{
    public record CreateRestaurantDto(string Name);
    public record CreateAddressDto(int RestaurantId, string Country, string City, string Street, string BuildingNumber);
    public record RestaurantWithAddressesDto(string Name, List<Address> addresses);
    public record RestaurantWithAddressDto(string Name, Address address);
    public record CreateFoodDto(int RestaurantId, int CategoryId, string FoodName, string Ingredients, int Price, int PreparationTime);
    public record FoodViewDto(int RestaurantId, int CategoryId, string FoodName, string Ingredients, int Price, int PreparationTime);
    public record CreateFoodCategoryDto(int RestaurantId, string CategoryName);
    public record FoodCategoryViewDto(int RestaurantId, string CategoryName);

}