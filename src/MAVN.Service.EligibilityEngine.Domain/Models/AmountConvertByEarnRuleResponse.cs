using System;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class AmountConvertByEarnRuleResponse : AmountConvertResponse
    {
        public Guid EarnRuleId { get; set; }
    }
}
