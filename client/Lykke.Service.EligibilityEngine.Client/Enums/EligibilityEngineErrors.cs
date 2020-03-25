namespace Lykke.Service.EligibilityEngine.Client.Enums
{
    /// <summary>
    /// Represents an enumeration of all errors returned by the Service.
    /// </summary>
    public enum EligibilityEngineErrors
    {
        /// <summary>
        /// No error.
        /// </summary>
        None,
        /// <summary>
        /// Partner was not found.
        /// </summary>
        PartnerNotFound,
        /// <summary>
        /// Customer was not found.
        /// </summary>
        CustomerNotFound,
        /// <summary>
        /// Earn rule was not found.
        /// </summary>
        EarnRuleNotFound,
        /// <summary>
        /// Spend rules was not found.
        /// </summary>
        SpendRuleNotFound,
        /// <summary>
        /// A conversion rate was not found.
        /// </summary>
        ConversionRateNotFound,
        /// <summary>
        /// Condition was not found
        /// </summary>
        ConditionNotFound
    }
}
