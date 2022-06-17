namespace Identity.Service.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public int YearOfFoundation { get; set; }
        public string CompanyLogin { get; set; }
        public string CompanyPassword { get; set; }
        public string CEO { get; set; }
    }
}
