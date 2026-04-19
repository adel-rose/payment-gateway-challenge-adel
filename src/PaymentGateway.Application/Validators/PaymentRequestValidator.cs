using System.Runtime.InteropServices.JavaScript;
using FluentValidation;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.Validators;

public sealed class PaymentRequestValidator : AbstractValidator<PaymentRequestDTO>
{
    public PaymentRequestValidator()
    {
        RuleFor(p => p.CardNumber)
            .NotNull()
            .NotEmpty()
            .MinimumLength(14)
            .MaximumLength(19)
            .Matches(@"^\d+$");

        RuleFor(p => p.ExpiryMonth)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(13);
        RuleFor(p => p.ExpiryYear)
            .NotNull()
            .NotEmpty();
        RuleFor(p => new DateTime(p.ExpiryYear, p.ExpiryMonth, 1))
            .GreaterThan(DateTime.Today);

        RuleFor(p => p.Currency)
            .Length(3);
        RuleFor(p => p.Currency)
            .Must(currency => Enum.GetNames<Currency>().Contains(currency))
            .WithMessage("Currency must be one of: " + string.Join(", ", Enum.GetNames<Currency>()));

        RuleFor(p => p.Amount)
            .NotNull()
            .NotEmpty();

        RuleFor(p => p.Cvv)
            .NotNull()
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(4)
            .Matches(@"^\d+$");
    }
}