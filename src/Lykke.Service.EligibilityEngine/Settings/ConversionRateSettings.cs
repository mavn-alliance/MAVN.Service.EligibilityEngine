using JetBrains.Annotations;

namespace Lykke.Service.EligibilityEngine.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ConversionRateSettings 
    {
        public string TokenCurrency { get; set; }

        public string BaseFiatCurrency { get; set; }
    }
}
