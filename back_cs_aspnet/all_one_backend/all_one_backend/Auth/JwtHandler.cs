using all_one_backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace all_one_backend.Auth
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;
        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string userName)
        {
            var tknHndlr = new JwtSecurityTokenHandler();
            var ky = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tknDscrpt = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(ky), SecurityAlgorithms.HmacSha256Signature)
            };

            var tkn = tknHndlr.CreateToken(tknDscrpt);

            return tknHndlr.WriteToken(tkn);
        }

        public JwtPayload DecodeToken(string token)
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            var ky = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tknHndlr = new JwtSecurityTokenHandler();
            var validParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(ky),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false
            };

            try
            {
                var prncpl = tknHndlr.ValidateToken(token, validParams, out var secuTkn);
                var JwtTkn = (JwtSecurityToken)secuTkn;

                return JwtTkn.Payload;
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"Validation error @ : {ex.Message}");
                return null;
            }
        }
    }
}
