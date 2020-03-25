using System;

namespace Lykke.Service.EligibilityEngine.Domain.Models
{
    public class AmountConvertByEarnRuleResponse : AmountConvertResponse
    {
        public Guid EarnRuleId { get; set; }
    }
}
