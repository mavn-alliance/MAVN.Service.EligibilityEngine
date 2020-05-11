using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.EligibilityEngine.Domain.Services
{
    public interface IRateCalculatorService
    {
        Task<Money18> CalculateConversionRate(decimal amountInCurrency, Money18 amountInTokens, string from, string to);
    }
}
