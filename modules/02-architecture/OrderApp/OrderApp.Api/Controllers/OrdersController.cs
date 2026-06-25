using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Application.Orders.Commands;
using OrderApp.Application.Orders.Queries;

namespace OrderApp.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ConfirmOrderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> ListByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var orders = await mediator.Send(new ListOrdersByCustomerQuery(customerId), cancellationToken);
        return Ok(orders);
    }
}
