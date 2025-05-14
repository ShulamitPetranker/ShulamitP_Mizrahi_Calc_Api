using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ShulamitP_Mizrahi_Calc_Api.Handlers.Arithmetic;
using ShulamitP_Mizrahi_Calc_Api.Models;
using ShulamitP_Mizrahi_Calc_Api.Dtos;
using Microsoft.Extensions.Logging;

namespace ShulamitP_Mizrahi_Calc_Api.Tests.Handlers
{
    public class ArithmeticHandlerTests
    {
        private readonly ArithmeticHandler _handler;
        private readonly Mock<ILogger<ArithmeticHandler>> _loggerMock;

        public ArithmeticHandlerTests()
        {
            _loggerMock = new Mock<ILogger<ArithmeticHandler>>();
            _handler = new ArithmeticHandler(_loggerMock.Object);
        }

        [Theory]
        [InlineData(10.0, 5.0, OperationType.Add, 15.0)]
        [InlineData(10.0, 5.0, OperationType.Subtract, 5.0)]
        [InlineData(10.0, 5.0, OperationType.Multiply, 50.0)]
        [InlineData(10.0, 5.0, OperationType.Divide, 2.0)]
        public void Calculate_ValidOperations_ReturnsCorrectResult(double num1, double num2, OperationType operation, double expected)
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = num1, Number2 = num2 },
                operation
            );

            // Act
            var result = _handler.Calculate(request);

            // Assert
            Assert.Equal(expected, result.Result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeast(2));
        }

        [Fact]
        public void Calculate_DivideByZero_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = 10, Number2 = 0 },
                OperationType.Divide
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _handler.Calculate(request));
            Assert.Equal("Cannot divide by zero.", exception.Message);
        }

        [Fact]
        public void Calculate_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new CalculateDto(null!, OperationType.Add);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _handler.Calculate(request));
        }

        [Fact]
        public void Calculate_NullNumbers_ThrowsArgumentException()
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = null, Number2 = null },
                OperationType.Add
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _handler.Calculate(request));
        }

        [Fact]
        public void Calculate_FirstNumberNull_ThrowsArgumentException()
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = null, Number2 = 10.0 },
                OperationType.Add
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _handler.Calculate(request));
        }

        [Fact]
        public void Calculate_SecondNumberNull_ThrowsArgumentException()
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = 10.0, Number2 = null },
                OperationType.Add
            );

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _handler.Calculate(request));
        }

        [Fact]
        public void Calculate_UnsupportedOperation_ThrowsNotSupportedException()
        {
            // Arrange
            var request = new CalculateDto(
                new CalculationRequest { Number1 = 10.0, Number2 = 5.0 },
                (OperationType)99 // Invalid operation
            );

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => _handler.Calculate(request));
        }
    }
} 