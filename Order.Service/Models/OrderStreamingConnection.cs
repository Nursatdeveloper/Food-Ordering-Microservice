namespace Order.Service.Models
{
    public class OrderStreamingConnection
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string RestaurantName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ConnectionPassword { get; set; }
    }
}
