using System.Text.RegularExpressions;
using FluentValidation;
using Shop.Application.DTOs.DeliveryAddress;

namespace Shop.Application.Validators.DeliveryAddress;

public class DeliveryAddressCreateValidator : AbstractValidator<DeliveryAddressCreateDto>
{
    public DeliveryAddressCreateValidator()
    {
        RuleFor(address => address.Title)
            .NotEmpty()
            .WithMessage("Назва адреси обов'язкова")
            .MaximumLength(100)
            .WithMessage("Назва адреси не може бути довшою за 100 символів");

        RuleFor(address => address.City)
            .NotEmpty()
            .WithMessage("Місто обов'язкове")
            .MaximumLength(100)
            .WithMessage("Назва міста не може бути довшою за 100 символів");

        RuleFor(address => address.Street)
            .NotEmpty()
            .WithMessage("Вулиця обов'язкова")
            .MaximumLength(150)
            .WithMessage("Назва вулиці не може бути довшою за 150 символів");

        RuleFor(address => address.House)
            .NotEmpty()
            .WithMessage("Номер будинку обов'язковий")
            .Must(IsValidHouseNumber)
            .WithMessage("Номер будинку має містити цифри та може мати літеру");

        RuleFor(address => address.PostalCode)
            .Must(IsValidPostalCode)
            .WithMessage("Поштовий індекс має містити 5 цифр")
            .When(address => !string.IsNullOrWhiteSpace(address.PostalCode));
    }

    private static bool IsValidHouseNumber(string house)
    {
        return Regex.IsMatch(house.Trim(), @"^\d+[А-Яа-яA-Za-z]?(?:[-/]\d+[А-Яа-яA-Za-z]?)?$");
    }

    private static bool IsValidPostalCode(string? postalCode)
    {
        return postalCode != null && Regex.IsMatch(postalCode.Trim(), @"^\d{5}$");
    }
}
