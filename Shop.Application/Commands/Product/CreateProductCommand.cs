using MediatR;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Commands.Product;

public record CreateProductCommand(ProductCreateDTO Dto) : IRequest<int>;