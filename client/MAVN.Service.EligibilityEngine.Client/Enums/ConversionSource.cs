namespace MAVN.Service.EligibilityEngine.Client.Enums
{
    /// <summary>
    /// Represents the source of the conversion rate.
    /// </summary>
    public enum ConversionSource
    {
        /// <summary>
        /// The conversion rate is taken from a burn rule.
        /// </summary>
        BurnRule,

        /// <summary>
        /// The conversion rate is taken from a earn rule.
        /// </summary>
        EarnRule,

        /// <summary>
        /// The conversion rate is taken from a partner profile.
        /// </summary>
        Partner,

        /// <summary>
        /// The conversion rate is taken from the global conversion rate.
        /// </summary>
        Global,
        /// <summary>
        /// The conversion rate is taken from a condition.
        /// </summary>
        Condition
    }
}
