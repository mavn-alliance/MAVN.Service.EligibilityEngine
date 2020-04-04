using System;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class AmountConvertBySpendRuleResponse : AmountConvertResponse
    {
        public Guid SpendRuleId { get; set; }
    }
}
