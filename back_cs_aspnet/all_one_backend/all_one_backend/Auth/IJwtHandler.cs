using System.IdentityModel.Tokens.Jwt;

namespace all_one_backend.Auth
{
    public interface IJwtHandler
    {
        string GenerateToken(int UserId, string UserName);
        JwtPayload DecodeToken(string Token);
    }
}
