using FluentValidation.TestHelper;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Validators;

namespace PaymentGateway.Tests.Api.ValidationTests;

public class PaymentRequestValidatorTests
{
    private readonly PaymentRequestValidator _sut = new();

    private static PaymentRequestDTO ValidDto() => new()
    {
        CardNumber = "4242424242424242",
        ExpiryMonth = 12,
        ExpiryYear = 2030,
        Currency = "GBP",
        Amount = 1000,
        Cvv = "123"
    };

    // CardNumber

    [Fact]
    public void CardNumber_When_null_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.CardNumber = null;
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Fact]
    public void CardNumber_When_empty_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.CardNumber = "";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Fact]
    public void CardNumber_When_less_than_14_digits_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.CardNumber = "1234567890123"; // 13 digits
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Fact]
    public void CardNumber_When_more_than_19_digits_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.CardNumber = "12345678901234567890"; // 20 digits
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Fact]
    public void CardNumber_When_contains_non_numeric_characters_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.CardNumber = "4242424242424ABC";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Fact]
    public void CardNumber_When_valid_Then_validation_passes()
    {
        var dto = ValidDto();
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
    }

    // ExpiryMonth

    [Fact]
    public void ExpiryMonth_When_zero_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 0;
        dto.ExpiryYear = 2030;
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.TestValidate(dto));
    }

    [Fact]
    public void ExpiryMonth_When_greater_than_12_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 13;
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.TestValidate(dto));
    }

    [Fact]
    public void ExpiryMonth_When_valid_Then_validation_passes()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 6;
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    // ExpiryYear

    [Fact]
    public void ExpiryYear_When_zero_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 1;
        dto.ExpiryYear = 0;
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.TestValidate(dto));
    }

    // Expiry combination

    [Fact]
    public void Expiry_When_combination_is_in_the_past_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 1;
        dto.ExpiryYear = 2020;
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => new DateTime(x.ExpiryYear, x.ExpiryMonth, 1));
    }

    [Fact]
    public void Expiry_When_combination_is_in_the_future_Then_validation_passes()
    {
        var dto = ValidDto();
        dto.ExpiryMonth = 12;
        dto.ExpiryYear = 2030;
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => new DateTime(x.ExpiryYear, x.ExpiryMonth, 1));
    }

    // Currency

    [Fact]
    public void Currency_When_not_3_characters_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Currency = "GB";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Currency_When_not_in_allowed_list_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Currency = "JPY";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData("GBP")]
    [InlineData("USD")]
    [InlineData("EUR")]
    public void Currency_When_valid_Then_validation_passes(string currency)
    {
        var dto = ValidDto();
        dto.Currency = currency;
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    // Amount

    [Fact]
    public void Amount_When_zero_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Amount = 0;
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_When_valid_Then_validation_passes()
    {
        var dto = ValidDto();
        dto.Amount = 1000;
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    // CVV

    [Fact]
    public void Cvv_When_null_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Cvv = null;
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Fact]
    public void Cvv_When_empty_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Cvv = "";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Fact]
    public void Cvv_When_less_than_3_digits_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Cvv = "12";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Fact]
    public void Cvv_When_more_than_4_digits_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Cvv = "12345";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Fact]
    public void Cvv_When_contains_non_numeric_characters_Then_validation_fails()
    {
        var dto = ValidDto();
        dto.Cvv = "12A";
        var result = _sut.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Cvv_When_valid_Then_validation_passes(string cvv)
    {
        var dto = ValidDto();
        dto.Cvv = cvv;
        var result = _sut.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Cvv);
    }
}