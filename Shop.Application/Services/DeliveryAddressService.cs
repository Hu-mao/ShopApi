using Shop.Application.DTOs.DeliveryAddress;
using Shop.Application.Interfaces.Repositories;
using Shop.Domain.Entities;

namespace Shop.Application.Services;

public class DeliveryAddressService
{
    private readonly IDeliveryAddressRepository _repository;

    public DeliveryAddressService(
        IDeliveryAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DeliveryAddressReadDto>> GetAllAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var addresses = await _repository.GetByUserIdAsync(
            userId,
            cancellationToken);

        return addresses.Select(x => new DeliveryAddressReadDto
        {
            Id = x.Id,
            Title = x.Title,
            City = x.City,
            Street = x.Street,
            House = x.House,
            Apartment = x.Apartment,
            PostalCode = x.PostalCode
        }).ToList();
    }

    public async Task<DeliveryAddressReadDto?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var address = await _repository.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (address == null)
            return null;

        return new DeliveryAddressReadDto
        {
            Id = address.Id,
            Title = address.Title,
            City = address.City,
            Street = address.Street,
            House = address.House,
            Apartment = address.Apartment,
            PostalCode = address.PostalCode
        };
    }

    public async Task<DeliveryAddressReadDto> CreateAsync(
        Guid userId,
        DeliveryAddressCreateDto dto,
        CancellationToken cancellationToken)
    {
        var address = new DeliveryAddress
        {
            UserId = userId,
            Title = dto.Title,
            City = dto.City,
            Street = dto.Street,
            House = dto.House,
            Apartment = dto.Apartment,
            PostalCode = dto.PostalCode
        };

        await _repository.AddAsync(
            address,
            cancellationToken);

        return new DeliveryAddressReadDto
        {
            Id = address.Id,
            Title = address.Title,
            City = address.City,
            Street = address.Street,
            House = address.House,
            Apartment = address.Apartment,
            PostalCode = address.PostalCode
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        Guid userId,
        DeliveryAddressUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var address = await _repository.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (address == null)
            return false;

        address.Title = dto.Title;
        address.City = dto.City;
        address.Street = dto.Street;
        address.House = dto.House;
        address.Apartment = dto.Apartment;
        address.PostalCode = dto.PostalCode;

        await _repository.UpdateAsync(
            address,
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var address = await _repository.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (address == null)
            return false;

        await _repository.DeleteAsync(
            address,
            cancellationToken);

        return true;
    }
}