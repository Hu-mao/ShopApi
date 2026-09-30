using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Repository
{
    public interface IDeliveryAddressRepository
    {
        Task<DeliveryAddress?> GetByIdAsync(
    int id,
    int userId,
    CancellationToken cancellationToken);

        Task<List<DeliveryAddress>> GetAllAsync(
            int userId,
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
}
