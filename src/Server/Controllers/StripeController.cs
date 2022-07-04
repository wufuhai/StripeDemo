using Microsoft.AspNetCore.Mvc;
using QNE.App.Server.Controllers;
using Stripe;
using StripeDemo.Shared;
using System.Collections.Generic;

namespace StripeDemo.Server.Controllers
{
    [Route("api/[controller]")]
    public class StripeController : Controller
    {
        [HttpPost("[action]")]
        public IActionResult Checkout([FromBody] StripeCheckoutModel model)
        {
            var domain = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";

            var priceOptions = new PriceListOptions
            {
                LookupKeys = new List<string> {
                    model.PlanKey
                }
            };
            var priceService = new PriceService();
            StripeList<Price> prices = priceService.List(priceOptions);

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                  new Stripe.Checkout.SessionLineItemOptions
                  {
                    Price = prices.Data[0].Id,
                    Quantity = 1,
                  },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/success/{CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/cancel/{CHECKOUT_SESSION_ID}",
            };
            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            return this.Success("Create stripe checkout success.", new StripeCheckoutResultModel { RedirectUrl = session.Url });
        }
    }
}
