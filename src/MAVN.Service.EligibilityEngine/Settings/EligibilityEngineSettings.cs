using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.EligibilityEngine.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EligibilityEngineSettings
    {
        public DbSettings Db { get; set; }

        public ConversionRateSettings ConversionRate { get; set; }
    }
}
