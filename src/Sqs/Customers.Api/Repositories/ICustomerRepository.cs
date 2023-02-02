using Customers.Api.Contracts.Data;

namespace Customers.Api.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();

    Task<CustomerDto?> GetAsync(Guid id);

    Task<bool> CreateAsync(CustomerDto customer);

    Task<bool> UpdateAsync(CustomerDto customer);

    Task<bool> DeleteAsync(Guid id);
}
