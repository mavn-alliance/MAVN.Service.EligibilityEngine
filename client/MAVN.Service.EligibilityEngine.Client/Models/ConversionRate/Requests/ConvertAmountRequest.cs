using System;
using MAVN.Numerics;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests
{
    /// <summary>
    /// A base class for any convert amount request.
    /// </summary>
    public class ConvertAmountRequest
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

        /// <summary>
        /// The amount for conversion.
        /// </summary>
        public Money18 Amount { get; set; }
    }
}
