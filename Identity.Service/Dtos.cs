namespace Identity.Service
{
    public class Dtos
    {
        public record UserLoginDto(string Telephone, string Password);
        public record UserRegisterDto(string FirstName, string LastName, DateTime BirthDate, string Telephone, string Password, string Role);
    }
}
