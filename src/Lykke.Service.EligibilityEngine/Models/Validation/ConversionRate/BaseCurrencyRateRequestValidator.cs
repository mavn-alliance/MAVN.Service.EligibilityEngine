using System;
using FluentValidation;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;

namespace Lykke.Service.EligibilityEngine.Models.Validation.ConversionRate
{
    public abstract class BaseCurrencyRateRequestValidator<T>: AbstractValidator<T>
        where T: CurrencyRateRequest
    {
        public BaseCurrencyRateRequestValidator()
        {
            RuleFor(p => p.FromCurrency)
                .NotNull()
                .NotEmpty()
                .WithMessage("From currency should not be null or empty.");

            RuleFor(p => p.ToCurrency)
                .NotNull()
                .NotEmpty()
                .WithMessage("To currency should not be null or empty.");

            RuleFor(p => p.CustomerId)
                .Must(p => p != Guid.Empty)
                .WithMessage("Customer Id must be provided and must not be an empty GUID.");
        }
    }
}
