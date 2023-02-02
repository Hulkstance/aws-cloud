using Customers.Api.Attributes;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using Customers.Api.Mapping;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Endpoints;

public static class CustomerEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Customers";
    private const string BaseRoute = "api/v1/customers";

    public static void MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, GetAllAsync)
            .WithName("GetCustomers")
            .Produces<IEnumerable<CustomerResponse>>()
            .WithTags(Tag);

        app.MapGet($"{BaseRoute}/{{id:guid}}", GetAsync)
            .WithName("GetCustomer")
            .Produces<CustomerResponse>().Produces(404)
            .WithTags(Tag);

        app.MapPost(BaseRoute, CreateAsync)
            .WithName("CreateCustomer")
            .Accepts<CustomerRequest>(ContentType)
            .Produces<CustomerResponse>(201).Produces<ValidationProblemDetails>(400)
            .WithTags(Tag);

        app.MapPut(BaseRoute, UpdateAsync)
            .WithName("UpdateCustomer")
            .Accepts<UpdateCustomerRequest>(ContentType)
            .Produces<CustomerResponse>().Produces<ValidationProblemDetails>(400)
            .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id:guid}}", DeleteAsync)
            .WithName("DeleteCustomer")
            .Produces(204).Produces(404)
            .WithTags(Tag);
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] ICustomerService customerService)
    {
        var customers = await customerService.GetAllAsync();
        var customersResponse = customers.ToCustomersResponse();
        return Results.Ok(customersResponse);
    }

    private static async Task<IResult> GetAsync(
        [FromRoute] Guid id,
        [FromServices] ICustomerService customerService)
    {
        var customer = await customerService.GetAsync(id);

        if (customer is null)
        {
            return Results.NotFound();
        }

        var customerResponse = customer.ToCustomerResponse();
        return Results.Ok(customerResponse);
    }

    private static async Task<IResult> CreateAsync(
        CustomerRequest request,
        ICustomerService customerService,
        CancellationToken cancellationToken)
    {
        var customer = request.ToCustomer();

        await customerService.CreateAsync(customer);

        var customerResponse = customer.ToCustomerResponse();

        return Results.CreatedAtRoute("GetCustomer", new { customerResponse.Id }, customerResponse);
    }

    private static async Task<IResult> UpdateAsync(
        [FromMultiSource] UpdateCustomerRequest request,
        [FromServices] ICustomerService customerService,
        CancellationToken cancellationToken)
    {
        var existingCustomer = await customerService.GetAsync(request.Id);

        if (existingCustomer is null)
        {
            return Results.NotFound();
        }

        var customer = request.ToCustomer();
        await customerService.UpdateAsync(customer);

        var customerResponse = customer.ToCustomerResponse();
        return Results.Ok(customerResponse);
    }

    private static async Task<IResult> DeleteAsync(
        [FromRoute] Guid id,
        [FromServices] ICustomerService customerService,
        CancellationToken cancellationToken)
    {
        var deleted = await customerService.DeleteAsync(id);
        if (!deleted)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
