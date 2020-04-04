using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// A request model for condition conversion rate.
    /// </summary>
    public class CurrencyRateByConditionRequest : CurrencyRateRequest
    {
        /// <summary>
        /// Contains condition id
        /// </summary>
        public Guid ConditionId { get; set; }
    }
}
