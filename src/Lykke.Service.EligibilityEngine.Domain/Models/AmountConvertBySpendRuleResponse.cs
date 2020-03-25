using System;

namespace Lykke.Service.EligibilityEngine.Domain.Models
{
    public class AmountConvertBySpendRuleResponse : AmountConvertResponse
    {
        public Guid SpendRuleId { get; set; }
    }
}
