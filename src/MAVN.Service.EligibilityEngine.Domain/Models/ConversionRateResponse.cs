using System;
using MAVN.Numerics;

namespace MAVN.Service.EligibilityEngine.Domain.Models
{
    public abstract class ConversionRateResponse
    {
        public Money18 UsedRate { get; set; }

        public ConversionSource ConversionSource { get; set; }

        public Guid CustomerId { get; set; }
    }
}
