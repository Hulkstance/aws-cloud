using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public sealed class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
{
    private readonly ILogger<CustomerDeletedHandler> _logger;

    public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
    {
        _logger = logger;
    }

    public Task<Unit> Handle(CustomerDeleted request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Id}", request.Id);
        return Unit.Task;
    }
}
