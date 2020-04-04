using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc />
    public class CurrencyRateByConditionResponse : CurrencyRateResponse
    {
        /// <summary>
        /// Represents condition id
        /// </summary>
        public Guid ConditionId { get; set; }
    }
}
