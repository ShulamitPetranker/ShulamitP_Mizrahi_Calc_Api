using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ShulamitP_Mizrahi_Calc_Api.Handlers.Authentication;

namespace ShulamitP_Mizrahi_Calc_Api.Tests.Handlers
{
    public class LoginHandlerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly LoginHandler _handler;
        private const string TestSecret = "TestSecretKey12345678901234567890";

        public LoginHandlerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns(TestSecret);
            
            _configurationMock.Setup(x => x.GetSection("Jwt:Secret"))
                            .Returns(configSectionMock.Object);

            _handler = new LoginHandler(_configurationMock.Object);
        }

        [Fact]
        public void GetJwt_ReturnsValidToken()
        {
            // Act
            var token = _handler.GetJwt();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Verify token claims
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "testuser");
            Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == "admin");
        }

        [Fact]
        public void GetJwt_MissingSecret_ThrowsArgumentNullException()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns((string)null);
            
            configMock.Setup(x => x.GetSection("Jwt:Secret"))
                     .Returns(configSectionMock.Object);
                     
            var handler = new LoginHandler(configMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => handler.GetJwt());
        }
    }
} 