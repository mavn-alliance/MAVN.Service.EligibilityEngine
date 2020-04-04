using Autofac;
using MAVN.Service.EligibilityEngine.Domain.Services;
using MAVN.Service.EligibilityEngine.DomainServices;
using MAVN.Service.EligibilityEngine.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.EligibilityEngine.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConversionRateService>()
                .As<IConversionRateService>()
                .SingleInstance();

            builder.RegisterType<RateCalculatorService>()
                .As<IRateCalculatorService>()
                .WithParameter("tokenCurrency", _appSettings.CurrentValue.EligibilityEngineService.ConversionRate.TokenCurrency)
                .WithParameter("baseFiatCurrency", _appSettings.CurrentValue.EligibilityEngineService.ConversionRate.BaseFiatCurrency)
                .SingleInstance();
        }
    }
}
