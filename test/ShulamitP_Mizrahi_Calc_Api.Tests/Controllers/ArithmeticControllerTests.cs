using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ShulamitP_Mizrahi_Calc_Api.Controllers;
using ShulamitP_Mizrahi_Calc_Api.Handlers.Arithmetic;
using ShulamitP_Mizrahi_Calc_Api.Models;
using ShulamitP_Mizrahi_Calc_Api.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ShulamitP_Mizrahi_Calc_Api.Tests.Controllers
{
    public class ArithmeticControllerTests
    {
        private readonly Mock<IArithmeticHandler> _arithmeticHandlerMock;
        private readonly Mock<ILogger<ArithmeticController>> _loggerMock;
        private readonly ArithmeticController _controller;

        public ArithmeticControllerTests()
        {
            _arithmeticHandlerMock = new Mock<IArithmeticHandler>();
            _loggerMock = new Mock<ILogger<ArithmeticController>>();
            _controller = new ArithmeticController(_arithmeticHandlerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Calculate_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new CalculationRequest { Number1 = 10, Number2 = 5 };
            var expectedResponse = new CalculationResponse { Result = 15 };
            
            _arithmeticHandlerMock.Setup(x => x.Calculate(It.IsAny<CalculateDto>()))
                                .Returns(expectedResponse);

            // Act
            var result = _controller.Calculate(request, OperationType.Add);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CalculationResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Result, response.Result);
            
            _arithmeticHandlerMock.Verify(x => x.Calculate(It.Is<CalculateDto>(dto => 
                dto.Request == request && 
                dto.Operation == OperationType.Add)), Times.Once);

            VerifyLoggerCalls();
        }

        [Fact]
        public void Calculate_HandlerThrowsArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var request = new CalculationRequest { Number1 = 10, Number2 = 0 };
            var errorMessage = "Both number1 and number2 must be provided.";
            
            _arithmeticHandlerMock.Setup(x => x.Calculate(It.IsAny<CalculateDto>()))
                                .Throws(new ArgumentException(errorMessage));

            // Act
            var result = _controller.Calculate(request, OperationType.Add);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorObj = JObject.FromObject(badRequestResult.Value);
            Assert.Equal(errorMessage, errorObj["error"].ToString());

            VerifyLoggerCalls();
        }

        [Fact]
        public void Calculate_HandlerThrowsInvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            var request = new CalculationRequest { Number1 = 10, Number2 = 0 };
            var errorMessage = "Cannot divide by zero.";
            
            _arithmeticHandlerMock.Setup(x => x.Calculate(It.IsAny<CalculateDto>()))
                                .Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = _controller.Calculate(request, OperationType.Divide);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            var errorObj = JObject.FromObject(objectResult.Value);
            Assert.Equal(errorMessage, errorObj["error"].ToString());

            VerifyLoggerCalls();
        }

        [Fact]
        public void Calculate_HandlerThrowsUnexpectedException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CalculationRequest { Number1 = 10, Number2 = 5 };
            
            _arithmeticHandlerMock.Setup(x => x.Calculate(It.IsAny<CalculateDto>()))
                                .Throws(new Exception("Unexpected error"));

            // Act
            var result = _controller.Calculate(request, OperationType.Add);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var errorObj = JObject.FromObject(statusCodeResult.Value);
            Assert.Equal("An unexpected error occurred.", errorObj["error"].ToString());

            VerifyLoggerCalls();
        }

        private void VerifyLoggerCalls()
        {
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeast(1));
        }
    }
} 