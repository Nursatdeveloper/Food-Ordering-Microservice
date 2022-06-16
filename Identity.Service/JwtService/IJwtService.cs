using Identity.Service.Models;

namespace Identity.Service.JwtService
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
