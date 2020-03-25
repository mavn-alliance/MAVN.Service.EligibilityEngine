using System;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <inheritdoc/>
    public class ConvertAmountByConditionResponse : ConvertAmountResponse
    {
        /// <summary>
        /// Represents condition id
        /// </summary>
        public Guid ConditionId { get; set; }
    }
}
