using System;
using Falcon.Numerics;
using Lykke.Service.EligibilityEngine.Client.Enums;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc/>
    public class ConvertAmountBySpendRuleResponse : ConvertAmountResponse
    {
        /// <summary>
        /// The Id of the spend rule.
        /// </summary>
        public Guid SpendRuleId { get; set; }
    }
}
