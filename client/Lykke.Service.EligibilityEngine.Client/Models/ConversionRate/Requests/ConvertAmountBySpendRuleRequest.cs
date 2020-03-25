using System;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for converting amount for given currencies based on spend rule.
    /// </summary>
    /// <inheritdoc/>
    public class ConvertAmountBySpendRuleRequest: ConvertAmountRequest
    {
        /// <summary>
        /// The id of the spend rule.
        /// </summary>
        public Guid SpendRuleId { get; set; }
    }
}
