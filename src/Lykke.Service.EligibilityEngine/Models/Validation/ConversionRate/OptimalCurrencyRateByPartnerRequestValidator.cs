using System;
using FluentValidation;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;

namespace Lykke.Service.EligibilityEngine.Models.Validation.ConversionRate
{
    public class OptimalCurrencyRateByPartnerRequestValidator : BaseCurrencyRateRequestValidator<OptimalCurrencyRateByPartnerRequest>
    {
        public OptimalCurrencyRateByPartnerRequestValidator() : base()
        {
            RuleFor(p => p.PartnerId)
                .Must(p => p != Guid.Empty)
                .WithMessage("Customer Id must be provided and must not be an empty GUID.");
        }
    }
}
