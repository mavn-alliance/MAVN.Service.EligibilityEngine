using System;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class OptimalAmountConvertResponse : AmountConvertResponse
    {
        public Guid? SpendRuleId { get; set; }
    }
}
