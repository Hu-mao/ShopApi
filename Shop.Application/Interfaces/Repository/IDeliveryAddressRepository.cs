using Shop.Domain.Entities;

namespace Shop.Application.Interfaces.Repositories;

public interface IDeliveryAddressRepository
{
    Task<List<DeliveryAddress>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<DeliveryAddress?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        DeliveryAddress address,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        DeliveryAddress address,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        DeliveryAddress address,
        CancellationToken cancellationToken);
}