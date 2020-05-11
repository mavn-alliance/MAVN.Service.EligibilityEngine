using System;
using MAVN.Numerics;
using MAVN.Service.EligibilityEngine.Client.Enums;

namespace MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Responses
{
    /// <summary>
    /// Represents an Response model of the conversion rate request.
    /// </summary>
    public abstract class CurrencyRateResponse : EligibilityEngineErrorResponseModel
    {
        /// <summary>
        /// The tokens rate.
        /// </summary>
        public Money18 Rate { get; set; }

        /// <summary>
        /// The source of the conversion rate.
        /// </summary>
        public ConversionSource ConversionSource { get; set; }
    }
}
