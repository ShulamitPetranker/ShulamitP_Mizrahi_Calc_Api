using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;

namespace ShulamitP_Mizrahi_Calc_Api.Security
{
    /// <summary>
    /// Handles bearer token authentication and JWT validation.
    /// </summary>
    public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Scheme name for authentication handler.
        /// </summary>
        public const string SchemeName = "Bearer";
        private readonly string _secretKey;

        /// <summary>
        /// Constructor for the authentication handler.
        /// </summary>
        public BearerAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _secretKey = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Key configuration is missing.");
        }

        /// <summary>
        /// Validates the JWT token from the Authorization header.
        /// </summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (!authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.Fail("Invalid Authorization Scheme");
                }

                var token = authHeader.Parameter;

                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for token expiration
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return AuthenticateResult.Fail("Invalid token");
                }

                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthenticateResult.Fail("Token has expired");
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
            }
        }
    }
}
