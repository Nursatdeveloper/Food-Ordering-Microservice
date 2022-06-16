namespace Identity.Service.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Telephone { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
