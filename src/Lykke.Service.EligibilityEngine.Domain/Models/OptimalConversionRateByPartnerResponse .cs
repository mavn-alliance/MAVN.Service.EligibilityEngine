using System;
using Falcon.Numerics;

namespace Lykke.Service.EligibilityEngine.Domain.Models
{
    public class OptimalConversionRateByPartnerResponse: ConversionRateResponse
    {
        public Guid? SpendRuleId { get; set; }
    }
}
