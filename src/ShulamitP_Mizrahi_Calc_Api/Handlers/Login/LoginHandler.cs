using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;

namespace ShulamitP_Mizrahi_Calc_Api.Handlers.Authentication
{
    public class LoginHandler : ILoginHandler
    {
        private readonly IConfiguration _configuration;

        public LoginHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetJwt()
        {
            var secretKey = _configuration.GetValue<string>(Consts.Consts.SECRET) ?? throw new ArgumentNullException("Jwt:Secret configuration is missing.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                new Claim("role", "admin")
                },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddYears(10),
                signingCredentials: creds
            );

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }
    }
}
