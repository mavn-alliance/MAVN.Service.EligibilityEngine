using Autofac;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.EligibilityEngine.Settings;
using Lykke.Service.PartnerManagement.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.EligibilityEngine.Modules
{
    public class ClientsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ClientsModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterCampaignClient(_appSettings.CurrentValue.CampaignServiceClient, null);

            builder.RegisterPartnerManagementClient(_appSettings.CurrentValue.PartnerManagementServiceClient, null);

            builder.RegisterCurrencyConvertorClient(_appSettings.CurrentValue.CurrencyConvertorServiceClient, null);
        }
    }
}
