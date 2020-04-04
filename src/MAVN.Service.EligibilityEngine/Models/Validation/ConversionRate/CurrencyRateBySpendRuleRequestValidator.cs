using System;
using FluentValidation;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;

namespace MAVN.Service.EligibilityEngine.Models.Validation.ConversionRate
{
    public class CurrencyRateBySpendRuleRequestValidator : BaseCurrencyRateRequestValidator<CurrencyRateBySpendRuleRequest>
    {
        public CurrencyRateBySpendRuleRequestValidator() : base()
        {
            RuleFor(p => p.SpendRuleId)
                .Must(p => p != Guid.Empty)
                .WithMessage("Customer Id must be provided and must not be an empty GUID.");
        }
    }
}
