using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.EligibilityEngine.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
