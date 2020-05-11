using System;
using MAVN.Numerics;
using MAVN.Service.EligibilityEngine.Client.Enums;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc/>
    public class ConvertOptimalByPartnerResponse : ConvertAmountResponse
    {
        /// <summary>
        /// The Id of the spend rule.
        /// </summary>
        public Guid? SpendRuleId { get; set; }
    }
}
