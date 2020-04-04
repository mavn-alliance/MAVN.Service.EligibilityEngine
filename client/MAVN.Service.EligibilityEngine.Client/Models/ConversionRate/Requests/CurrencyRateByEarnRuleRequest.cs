using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for getting a conversion rate based on spend rule.
    /// </summary>
    /// <inheritdoc/>
    public class CurrencyRateBySpendRuleRequest : CurrencyRateRequest
    {
        /// <summary>
        /// The id of the spend rule.
        /// </summary>
        public Guid SpendRuleId { get; set; }
    }
}
