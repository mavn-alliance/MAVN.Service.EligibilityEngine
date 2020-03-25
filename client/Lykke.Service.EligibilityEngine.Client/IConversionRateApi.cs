using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Responses;
using Refit;

namespace Lykke.Service.EligibilityEngine.Client
{
    /// <summary>
    /// Conversion rate API interface.
    /// </summary>
    [PublicAPI]
    public interface IConversionRateApi
    {
        /// <summary>
        /// Gets an appropriate conversion rate for partner.
        /// </summary>
        /// <param name="request"><see cref="OptimalCurrencyRateByPartnerRequest"/></param>
        /// <returns><see cref="OptimalCurrencyRateByPartnerResponse"/></returns>
        [Post("/api/conversionRate/partnerRate")]
        Task<OptimalCurrencyRateByPartnerResponse> GetOptimalCurrencyRateByPartnerAsync(OptimalCurrencyRateByPartnerRequest request);

        /// <summary>
        /// Gets a conversion rate from earn rule.
        /// </summary>
        /// <param name="request"><see cref="CurrencyRateByEarnRuleRequest"/></param>
        /// <returns><see cref="CurrencyRateByEarnRuleResponse"/></returns>
        [Post("/api/conversionRate/earnRuleRate")]
        Task<CurrencyRateByEarnRuleResponse> GetCurrencyRateByEarnRuleIdAsync(CurrencyRateByEarnRuleRequest request);

        /// <summary>
        /// Gets a conversion rate from spend rule.
        /// </summary>
        /// <param name="request"><see cref="CurrencyRateBySpendRuleRequest"/></param>
        /// <returns><see cref="CurrencyRateBySpendRuleResponse"/></returns>
        [Post("/api/conversionRate/spendRuleRate")]
        Task<CurrencyRateBySpendRuleResponse> GetCurrencyRateBySpendRuleIdAsync(CurrencyRateBySpendRuleRequest request);

        /// <summary>
        /// Converts given amount from one currency to another for partner.
        /// </summary>
        /// <param name="request"><see cref="ConvertOptimalByPartnerRequest"/></param>
        /// <returns><see cref="ConvertOptimalByPartnerResponse"/></returns>
        [Post("/api/conversionRate/partnerAmount")]
        Task<ConvertOptimalByPartnerResponse> ConvertOptimalByPartnerAsync(ConvertOptimalByPartnerRequest request);

        /// <summary>
        /// Converts given amount from one currency to another for earn rule.
        /// </summary>
        /// <param name="request"><see cref="ConvertAmountByEarnRuleRequest"/></param>
        /// <returns><see cref="ConvertAmountByEarnRuleResponse"/></returns>
        [Post("/api/conversionRate/earnRuleAmount")]
        Task<ConvertAmountByEarnRuleResponse> GetAmountByEarnRuleAsync(ConvertAmountByEarnRuleRequest request);

        /// <summary>
        /// Converts given amount from one currency to another for spend rule.
        /// </summary>
        /// <param name="request"><see cref="ConvertAmountBySpendRuleRequest"/></param>
        /// <returns><see cref="ConvertAmountBySpendRuleResponse"/></returns>
        [Post("/api/conversionRate/spendRuleAmount")]
        Task<ConvertAmountBySpendRuleResponse> GetAmountBySpendRuleAsync(ConvertAmountBySpendRuleRequest request);

        /// <summary>
        /// Converts given amount from one currency to another for a condition rule.
        /// </summary>
        /// <param name="request"><see cref="ConvertAmountByConditionRequest"/></param>
        /// <returns><see cref="ConvertAmountByConditionResponse"/></returns>
        [Post("/api/conversionRate/conditionAmount")]
        Task<ConvertAmountByConditionResponse> GetAmountByConditionAsync(ConvertAmountByConditionRequest request);

        /// <summary>
        /// Gets a conversion rate from a condition.
        /// </summary>
        /// <param name="request"><see cref="CurrencyRateByConditionRequest"/></param>
        /// <returns><see cref="CurrencyRateByConditionResponse"/></returns>
        [Post("/api/conversionRate/conditionRate")]
        Task<CurrencyRateByConditionResponse> GetCurrencyRateByConditionIdAsync(CurrencyRateByConditionRequest request);
    }
}
