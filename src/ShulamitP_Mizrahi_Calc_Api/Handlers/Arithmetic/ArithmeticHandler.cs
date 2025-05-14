using Microsoft.Extensions.Logging;
using ShulamitP_Mizrahi_Calc_Api.Controllers;
using ShulamitP_Mizrahi_Calc_Api.Dtos;
using ShulamitP_Mizrahi_Calc_Api.Models;
using System;
using System.Threading.Tasks;

namespace ShulamitP_Mizrahi_Calc_Api.Handlers.Arithmetic
{
    /// <summary>
    /// Handles arithmetic operations based on the provided request.
    /// </summary>
    public class ArithmeticHandler : IArithmeticHandler
    {
        private readonly ILogger<ArithmeticHandler> _logger;

        public ArithmeticHandler(ILogger<ArithmeticHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Performs an arithmetic calculation based on the given request.
        /// </summary>
        /// <param name="request">The calculation request, including the operation and operands.</param>
        /// <returns>A task representing the asynchronous operation, containing the calculation result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the request body is null.</exception>
        /// <exception cref="ArgumentException">Thrown when one or both operands are missing, or division by zero is attempted.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported operation is provided.</exception>
        public CalculationResponse Calculate(CalculateDto request)
        {
            try
            {
                _logger.LogInformation("Starting calculation. Operation: {Operation}, Number1: {Number1}, Number2: {Number2}",
    request.Operation, request.Request?.Number1, request.Request?.Number2);

                if (request.Request == null)
                {
                    _logger.LogWarning("Request body is null.");
                    throw new ArgumentNullException(nameof(request.Request), "Request body cannot be null.");
                }

                if (request.Request.Number1 == null || request.Request.Number2 == null)
                {
                    _logger.LogWarning("One or both numbers are missing in the request.");
                    throw new ArgumentException("Both number1 and number2 must be provided.");
                }

                double number1 = request.Request.Number1.Value;
                double number2 = request.Request.Number2.Value;

                double result = request.Operation switch
                {
                    OperationType.Add => number1 + number2,
                    OperationType.Subtract => number1 - number2,
                    OperationType.Multiply => number1 * number2,
                    OperationType.Divide => number2 != 0
                        ? number1 / number2
                        : throw new InvalidOperationException("Cannot divide by zero."),
                    _ => throw new NotSupportedException($"Unsupported operation: {request.Operation}")
                };

                _logger.LogInformation("Calculation successful. Result: {Result}", result);

                return new CalculationResponse { Result = result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during calculation.");
                throw;
            }
        }
    }
}
