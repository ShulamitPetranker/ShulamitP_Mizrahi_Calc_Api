using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ShulamitP_Mizrahi_Calc_Api.Controllers;
using ShulamitP_Mizrahi_Calc_Api.Handlers.Authentication;
using ShulamitP_Mizrahi_Calc_Api.Models;

namespace ShulamitP_Mizrahi_Calc_Api.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<ILoginHandler> _loginHandlerMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _loginHandlerMock = new Mock<ILoginHandler>();
            _controller = new AuthenticationController(_loginHandlerMock.Object);
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsOkResultWithToken()
        {
            // Arrange
            var expectedToken = "valid.jwt.token";
            _loginHandlerMock.Setup(x => x.GetJwt())
                           .Returns(expectedToken);

            // Act
            var result = await _controller.Login();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as string;
            Assert.Equal(expectedToken, response);
            
            _loginHandlerMock.Verify(x => x.GetJwt(), Times.Once);
        }

        [Fact]
        public async Task Login_HandlerReturnsNull_ReturnsUnauthorized()
        {
            // Arrange
            _loginHandlerMock.Setup(x => x.GetJwt())
                           .Returns((string)null);

            // Act
            var result = await _controller.Login();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
            
            _loginHandlerMock.Verify(x => x.GetJwt(), Times.Once);
        }
    }
} 