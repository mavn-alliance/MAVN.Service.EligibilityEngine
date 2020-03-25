using Lykke.HttpClientGenerator;

namespace Lykke.Service.EligibilityEngine.Client
{
    /// <summary>
    /// EligibilityEngine API aggregating interface.
    /// </summary>
    public class EligibilityEngineClient : IEligibilityEngineClient
    {
        // Note: Add similar ConversionRate properties for each new service controller

        /// <summary>Interface to Conversion rate ConversionRate.</summary>
        public IConversionRateApi ConversionRate { get; private set; }

        /// <summary>C-tor</summary>
        public EligibilityEngineClient(IHttpClientGenerator httpClientGenerator)
        {
            ConversionRate = httpClientGenerator.Generate<IConversionRateApi>();
        }
    }
}
