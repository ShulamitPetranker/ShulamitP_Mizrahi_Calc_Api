using ShulamitP_Mizrahi_Calc_Api.Controllers;
using ShulamitP_Mizrahi_Calc_Api.Dtos;
using ShulamitP_Mizrahi_Calc_Api.Models;
using System.Threading.Tasks;

namespace ShulamitP_Mizrahi_Calc_Api.Handlers.Arithmetic
{
    public interface IArithmeticHandler
    {
        public CalculationResponse Calculate(CalculateDto request);
    }
}
