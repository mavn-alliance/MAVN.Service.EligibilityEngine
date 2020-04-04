using System;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// A base class for any conversion rate request.
    /// </summary>
    public class CurrencyRateRequest
    {
        /// <summary>
        /// From which currency is conversation from.
        /// </summary>
        public string FromCurrency { get; set; }

        /// <summary>
        /// To which currency is conversation from.
        /// </summary>
        public string ToCurrency { get; set; }

        /// <summary>
        /// The id of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }
    }
}
