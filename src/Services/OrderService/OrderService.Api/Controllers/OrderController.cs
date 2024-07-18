using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Queries.GetOrderDeteailById;
using System.Net;

namespace OrderService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IMediator _mediator) : ControllerBase
{  
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOrderDetailsById(Guid id)
    {
        var result = await _mediator.Send(new GetOrderDetailsQuery(id));

        return Ok(result);
    }
}
