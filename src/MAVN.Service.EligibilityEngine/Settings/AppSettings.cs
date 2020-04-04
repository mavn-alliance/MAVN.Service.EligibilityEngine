using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.PartnerManagement.Client;

namespace MAVN.Service.EligibilityEngine.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public EligibilityEngineSettings EligibilityEngineService { get; set; }

        public CampaignServiceClientSettings CampaignServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }

        public CurrencyConvertorServiceClientSettings CurrencyConvertorServiceClient { get; set; }
    }
}
