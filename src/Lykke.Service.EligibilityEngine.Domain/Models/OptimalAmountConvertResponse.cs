using System;

namespace Lykke.Service.EligibilityEngine.Domain.Models
{
    public class OptimalAmountConvertResponse : AmountConvertResponse
    {
        public Guid? SpendRuleId { get; set; }
    }
}
