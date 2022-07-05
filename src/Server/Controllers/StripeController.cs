using Microsoft.AspNetCore.Mvc;
using QNE.App.Server.Controllers;
using Stripe;
using StripeDemo.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        [HttpPost("[action]")]
        public IActionResult ManageBilling([FromBody] ManageBillingModel model)
        {
            try
            {
                // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
                // Typically this is stored alongside the authenticated user in your database.
                var checkoutService = new Stripe.Checkout.SessionService();
                var checkoutSession = checkoutService.Get(model.SessionId);

                // This is the URL to which your customer will return after
                // they are done managing billing in the Customer Portal.
                var returnUrl = "http://localhost:4242";

                var options = new Stripe.BillingPortal.SessionCreateOptions
                {
                    Customer = checkoutSession.CustomerId,
                    ReturnUrl = returnUrl,
                };
                var service = new Stripe.BillingPortal.SessionService();
                var session = service.Create(options);
                return this.Success("Get stripe billing success.", new ManageBillingResultModel { RedirectUrl = session.Url });
            }
            catch (StripeException ex)
            {
                return this.Error("ERR_STRIPE_EX", ex.Message);
            }
        }

        [HttpPost("webhook/[action]")]
        public async Task<IActionResult> OnSubscriptionChanged()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            // Replace this endpoint secret with your endpoint's unique secret
            // If you are testing with the CLI, find the secret by running 'stripe listen'
            // If you are using an endpoint defined with the API or dashboard, look in your webhook settings
            // at https://dashboard.stripe.com/webhooks
            const string endpointSecret = "whsec_d8c62633acb7534410891b32da63757a6bee4fd245f61794da14fd425cdcac4e";
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret, throwOnApiVersionMismatch: false);
                if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    Console.WriteLine("A subscription was canceled.", subscription.Id);
                    // Then define and call a method to handle the successful payment intent.
                    // handleSubscriptionCanceled(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    Console.WriteLine("A subscription was updated.", subscription.Id);
                    // Then define and call a method to handle the successful payment intent.
                    // handleSubscriptionUpdated(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    Console.WriteLine("A subscription was created.", subscription.Id);
                    // Then define and call a method to handle the successful payment intent.
                    // handleSubscriptionUpdated(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionTrialWillEnd)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    Console.WriteLine("A subscription trial will end", subscription.Id);
                    // Then define and call a method to handle the successful payment intent.
                    // handleSubscriptionUpdated(subscription);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
        }
    }
}
