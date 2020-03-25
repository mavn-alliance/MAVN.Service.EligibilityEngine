using System;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for converting amount for given currencies based on partner.
    /// </summary>
    /// <inheritdoc/>
    public class ConvertOptimalByPartnerRequest: ConvertAmountRequest
    {
        /// <summary>
        /// The id of the partner.
        /// </summary>
        public Guid PartnerId { get; set; }
    }
}
