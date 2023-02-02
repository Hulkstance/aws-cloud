using Customers.Api.Domain;

namespace Customers.Api.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetAsync(Guid id);

    Task<bool> CreateAsync(Customer customer);

    Task<bool> UpdateAsync(Customer customer);

    Task<bool> DeleteAsync(Guid id);
}
