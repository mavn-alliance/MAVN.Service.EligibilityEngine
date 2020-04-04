using System;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class ConversionRateByEarnRuleResponse : ConversionRateResponse
    {
        public Guid EarnRuleId { get; set; }
    }
}
