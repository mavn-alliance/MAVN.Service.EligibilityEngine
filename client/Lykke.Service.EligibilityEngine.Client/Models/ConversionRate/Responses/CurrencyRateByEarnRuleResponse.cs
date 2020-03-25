using System;
using Falcon.Numerics;
using Lykke.Service.EligibilityEngine.Client.Enums;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc/>
    public class CurrencyRateByEarnRuleResponse : CurrencyRateResponse
    {
        /// <summary>
        /// The Id of the earn rule.
        /// </summary>
        public Guid EarnRuleId { get; set; }
    }
}
