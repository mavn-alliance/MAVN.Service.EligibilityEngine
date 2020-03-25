using JetBrains.Annotations;

namespace Lykke.Service.EligibilityEngine.Client
{
    /// <summary>
    /// EligibilityEngine client interface.
    /// </summary>
    [PublicAPI]
    public interface IEligibilityEngineClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - IConversionRateApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application ConversionRate interface</summary>
        IConversionRateApi ConversionRate { get; }
    }
}
