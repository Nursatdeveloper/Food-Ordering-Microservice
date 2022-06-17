namespace Identity.Service
{
    public class Dtos
    {
        public record UserLoginDto(string Telephone, string Password);
        public record UserRegisterDto(string FirstName, string LastName, DateTime BirthDate, string Telephone, string Password, string Role);
        public record CompanyLoginDto(string companyLogin, string companyPassword);
        public record CompanyRegisterDto(string CompanyName, int YearOfFoundation, string CompanyLogin, string CompanyPassword, string CEO);
    }
}
