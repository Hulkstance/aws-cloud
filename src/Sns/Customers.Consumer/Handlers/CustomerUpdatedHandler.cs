using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public sealed class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
{
    private readonly ILogger<CustomerUpdatedHandler> _logger;

    public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(CustomerUpdated request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{GitHubUsername}", request.GitHubUsername);
        return Unit.Task;
    }
}
