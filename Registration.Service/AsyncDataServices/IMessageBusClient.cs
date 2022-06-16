using Registration.Service.PublishItems;

namespace Registration.Service.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishRestaurant(PublishRestaurant publishRestaurant);
        void PublishRestaurantAddress(PublishRestaurantAddress publishRestaurantAddress);
        void PublishFoodCategory(PublishFoodCategory publishFoodCategory);
        void PublishFood(PublishFood publishFood);

    }
}