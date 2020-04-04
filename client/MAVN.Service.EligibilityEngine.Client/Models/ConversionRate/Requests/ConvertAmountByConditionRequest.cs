using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <inheritdoc/>
    public class ConvertAmountByConditionRequest : ConvertAmountRequest
    {
        /// <summary>
        /// Represents condition id
        /// </summary>
        public Guid ConditionId { get; set; }
    }
}
