using System;
using MAVN.Numerics;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class ConversionRateBySpendRuleResponse: ConversionRateResponse
    {
        public Guid SpendRuleId { get; set; }
    }
}
