using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for converting amount for given currencies based on earn rule.
    /// </summary>
    /// <inheritdoc/>
    public class ConvertAmountByEarnRuleRequest: ConvertAmountRequest
    {
        /// <summary>
        /// The id of the earn rule.
        /// </summary>
        public Guid EarnRuleId { get; set; }
    }
}
