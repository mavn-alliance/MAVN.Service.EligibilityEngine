using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Service.EligibilityEngine.Domain.Services
{
    public interface IRateCalculatorService
    {
        Task<Money18> CalculateConversionRate(decimal amountInCurrency, Money18 amountInTokens, string from, string to);
    }
}
