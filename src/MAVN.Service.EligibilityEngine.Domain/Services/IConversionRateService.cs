using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.EligibilityEngine.Domain.Models;

namespace MAVN.Service.EligibilityEngine.Domain.Services
{
    public interface IConversionRateService
    {
        Task<OptimalConversionRateByPartnerResponse> GetOptimalCurrencyRateByPartnerAsync(Guid partnerId, Guid customerId, string from, string to);
        Task<ConversionRateByEarnRuleResponse> GetCurrencyRateByEarnRuleIdAsync(Guid requestEarnRuleId, Guid requestCustomerId, string from, string to);
        Task<ConversionRateBySpendRuleResponse> GetCurrencyRateBySpendRuleIdAsync(Guid requestSpendRuleId, Guid requestCustomerId, string from, string to);
        Task<OptimalAmountConvertResponse> ConvertOptimalByPartnerAsync(Guid partnerId, Guid customerId, string from, string to, Money18 amount);
        Task<AmountConvertByEarnRuleResponse> GetAmountByEarnRuleAsync(Guid earnRuleId, Guid customerId, string from, string to, Money18 amount);
        Task<AmountConvertBySpendRuleResponse> GetAmountBySpendRuleAsync(Guid spendRuleId, Guid customerId, string from, string to, Money18 amount);
        Task<AmountByConditionResponse> GetAmountConditionAsync(Guid conditionId, Guid customerId, string fromCurrency, string toCurrency, Money18 amount);
        Task<ConversionRateByConditionResponse> GetCurrencyRateByConditionIdAsync(Guid conditionId, Guid customerId, string fromCurrency, string toCurrency);
    }
}
