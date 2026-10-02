using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repositories;
using Shop.Domain.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class DeliveryAddressRepository : IDeliveryAddressRepository
{
    private readonly ShopDbContext _context;

    public DeliveryAddressRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeliveryAddress>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.DeliveryAddresses
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeliveryAddress?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.DeliveryAddresses
            .FirstOrDefaultAsync(
                x => x.Id == id && x.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
    DeliveryAddress address,
    CancellationToken cancellationToken)
    {
        await _context.DeliveryAddresses.AddAsync(
            address,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        DeliveryAddress address,
        CancellationToken cancellationToken)
    {
        _context.DeliveryAddresses.Update(address);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        DeliveryAddress address,
        CancellationToken cancellationToken)
    {
        _context.DeliveryAddresses.Remove(address);

        await _context.SaveChangesAsync(cancellationToken);
    }
}