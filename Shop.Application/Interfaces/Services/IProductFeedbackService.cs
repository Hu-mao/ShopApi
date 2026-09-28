using Shop.Application.DTOs.ProductFeedbackDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IProductFeedbackService
{
    Task AddAsync(int productId, ProductFeedbackCreateDTO dto);
}