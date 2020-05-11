using System;
using MAVN.Numerics;
using MAVN.Service.EligibilityEngine.Client.Enums;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc/>
    public class ConvertAmountByEarnRuleResponse : ConvertAmountResponse
    {
        /// <summary>
        /// The Id of the earn rule.
        /// </summary>
        public Guid EarnRuleId { get; set; }
    }
}
