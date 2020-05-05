using Autofac;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.EligibilityEngine.Settings;
using MAVN.Service.PartnerManagement.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.EligibilityEngine.Modules
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
