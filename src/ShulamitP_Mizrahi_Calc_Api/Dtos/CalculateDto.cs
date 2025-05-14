using ShulamitP_Mizrahi_Calc_Api.Models;
namespace ShulamitP_Mizrahi_Calc_Api.Dtos;

/// <summary>
/// Represents a data transfer object for performing an arithmetic operation.
/// </summary>
/// <param name="Request">The request containing the two numeric values.</param>
/// <param name="Operation">The type of arithmetic operation to perform (e.g., Add, Subtract, Multiply, Divide).</param>
public record CalculateDto(CalculationRequest Request, OperationType Operation);
