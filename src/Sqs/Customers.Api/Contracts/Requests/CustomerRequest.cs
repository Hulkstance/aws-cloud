namespace Customers.Api.Contracts.Requests;

public class CustomerRequest
{
    public required string GitHubUsername { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required DateTime DateOfBirth { get; init; }
}
