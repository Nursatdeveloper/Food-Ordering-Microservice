namespace Registration.Service.PublishItems
{
    public class PublishRestaurantAddress
    {
            public record PublishRestaurantAddressDto(int RestaurantId, string Country, string City, string Street, string BuildingNumber, string Event);
        public string? RestaurantName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? Event { get; set; }
    }
}