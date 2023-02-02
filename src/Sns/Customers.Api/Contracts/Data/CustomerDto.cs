namespace Customers.Api.Contracts.Data;

public class CustomerDto
{
    public required Guid Id { get; init; }

    public required string GitHubUsername { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required DateTime DateOfBirth { get; init; }
}
