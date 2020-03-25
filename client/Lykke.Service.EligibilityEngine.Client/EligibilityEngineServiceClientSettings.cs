using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.EligibilityEngine.Client 
{
    /// <summary>
    /// EligibilityEngine client settings.
    /// </summary>
    public class EligibilityEngineServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
