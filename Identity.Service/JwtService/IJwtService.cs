using Identity.Service.Models;

namespace Identity.Service.JwtService
{
    public interface IJwtService
    {
        string GenerateTokenForUser(User user);
        string GenerateTokenForCompany(Company company);
    }
}
