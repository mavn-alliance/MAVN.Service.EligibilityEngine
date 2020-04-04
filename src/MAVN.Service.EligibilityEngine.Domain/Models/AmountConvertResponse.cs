using System;
using Falcon.Numerics;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public class AmountConvertResponse
    {
        public Money18 Amount { get; set; }

        public string CurrencyCode { get; set; }

        public Money18 UsedRate { get; set; }

        public ConversionSource ConversionSource { get; set; }

        public Guid CustomerId { get; set; }
    }
}
