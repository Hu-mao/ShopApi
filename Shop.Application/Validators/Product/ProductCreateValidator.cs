using FluentValidation;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Validators.Product;

public class ProductCreateValidator : AbstractValidator<ProductCreateDTO>
{
    public ProductCreateValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty()
            .WithMessage("Назва товару обов'язкова")
            .Must(name => name.Trim().Length >= 3)
            .WithMessage("Назва товару повинна містити щонайменше 3 символи");

        RuleFor(product => product.Price)
            .GreaterThan(0)
            .WithMessage("Ціна товару повинна бути більшою за 0");

        RuleFor(product => product.StockQty)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Кількість товару не може бути від'ємною");

        RuleFor(product => product.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId повинен бути більшим за 0");

        RuleFor(product => product.Images)
            .Must(images => images.Count <= 10)
            .WithMessage("Можна додати не більше 10 зображень");
    }
}
