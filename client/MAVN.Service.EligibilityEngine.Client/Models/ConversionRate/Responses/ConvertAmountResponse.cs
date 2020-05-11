using System;
using MAVN.Numerics;
using MAVN.Service.EligibilityEngine.Client.Enums;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <summary>
    /// Represents an Response model of the converting amount response.
    /// </summary>
    public abstract class ConvertAmountResponse : EligibilityEngineErrorResponseModel
    {
        /// <summary>
        /// The amount.
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// The currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The tokens rate.
        /// </summary>
        public Money18 UsedRate { get; set; }

        /// <summary>
        /// The source of the conversion rate.
        /// </summary>
        public ConversionSource ConversionSource { get; set; }
    }
}
