using System;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for getting a conversion rate based on earn rule.
    /// </summary>
    /// <inheritdoc/>
    public class CurrencyRateByEarnRuleRequest: CurrencyRateRequest
    {
        /// <summary>
        /// The id of the earn rule.
        /// </summary>
        public Guid EarnRuleId { get; set; }
    }
}
