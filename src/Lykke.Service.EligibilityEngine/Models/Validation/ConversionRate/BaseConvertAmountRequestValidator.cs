using System;
using FluentValidation;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;

namespace Lykke.Service.EligibilityEngine.Models.Validation.ConversionRate
{
    public abstract class BaseConvertAmountRequestValidator<T>: AbstractValidator<T>
        where T: ConvertAmountRequest
    {
        public BaseConvertAmountRequestValidator()
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

            RuleFor(r => r.Amount)
                .NotNull()
                .Must(r => r >= 0)
                .WithMessage("Amount should be valid number equal ot greater than 0.");
        }
    }
}
