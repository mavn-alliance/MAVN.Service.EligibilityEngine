using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.EligibilityEngine.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
