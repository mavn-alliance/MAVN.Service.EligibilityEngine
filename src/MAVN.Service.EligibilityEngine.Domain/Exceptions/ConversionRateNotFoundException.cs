using System;

namespace MAVN.Service.EligibilityEngine.Domain.Exceptions
{
    public class ConversionRateNotFoundException: Exception
    {
        public ConversionRateNotFoundException(string message) : base(message)
        {
        }
    }
}
