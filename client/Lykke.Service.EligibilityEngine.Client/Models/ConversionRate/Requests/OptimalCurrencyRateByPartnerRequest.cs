using System;

namespace Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// Represents a request model for getting the best conversion rate based on partner.
    /// </summary>
    /// <inheritdoc/>
    public class OptimalCurrencyRateByPartnerRequest: CurrencyRateRequest
    {
        /// <summary>
        /// The id of the partner.
        /// </summary>
        public Guid PartnerId { get; set; }
    }
}
