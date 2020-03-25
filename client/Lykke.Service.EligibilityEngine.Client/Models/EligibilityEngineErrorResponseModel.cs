using Lykke.Service.EligibilityEngine.Client.Enums;

namespace Lykke.Service.EligibilityEngine.Client.Models
{
    /// <summary>
    /// Represents a response model containing error information.
    /// </summary>
    public class EligibilityEngineErrorResponseModel
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public EligibilityEngineErrors ErrorCode { get; set; }

        /// <summary>
        /// The Error Message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
