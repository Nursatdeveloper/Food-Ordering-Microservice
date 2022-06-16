namespace Catalog.Service.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
    }
}