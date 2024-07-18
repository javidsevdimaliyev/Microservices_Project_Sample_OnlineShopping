using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using EventBus.Base.Abstraction;
using EventBus.Shared.Events.Order;
using EventBus.Shared.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utilities;

namespace BasketService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        IBasketRepository basketRepository, 
        IEventBus eventBus, 
        ILogger<BasketController> logger)
    {
        _basketRepository = basketRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult Get()
    {
        return Ok("Basket Service is App and Running!");
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBaskeyAsync([FromBody] CustomerBasket basket)
    {
        return Ok(await _basketRepository.UpdateBasketAsync(basket));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem item)
    {
        var userId = IdentityHelper.GetUserId().ToString();

        var basket = await _basketRepository.GetBasketAsync(userId);

        if (basket == null)
        {
            basket = new CustomerBasket(userId);
        }

        basket.Items.Add(item);

        await _basketRepository.UpdateBasketAsync(basket);

        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var buyerId = basketCheckout.BuyerId;

        var basket = await _basketRepository.GetBasketAsync(buyerId.ToString());

        if (basket == null)
        {
            return BadRequest();
        }

        //Event Publishing 
        var eventMessage = new OrderPreparedIntegrationEvent(
            buyerId, basketCheckout.City, 
            basketCheckout.Street, basketCheckout.State, basketCheckout.Country, 
            basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName, 
            basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.CardExpiration, basket.Items.Map<OrderItemMessage>().ToList());

        try
        {
            // Listens itself to clean the basket
            // It is listened by OrderApi to start the process
            await _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERROR: Publishing integration event with ID: {eventMessage.Id} from BasketService");

            throw;
        }

        return Accepted();
    }

    // DELETE -> api/v1/[controller]?{id=1}
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeteleteBasketByIdAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}
