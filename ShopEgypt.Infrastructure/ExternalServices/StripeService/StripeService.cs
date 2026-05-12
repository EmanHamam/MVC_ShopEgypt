using ShopEgypt.Application.Interfaces.IStripeService;
using Stripe;

namespace ShopEgypt.Infrastructure.ExternalServices.StripeService
{
    /// <summary>
    /// Stripe API key is set globally at startup via StripeConfiguration.ApiKey
    /// in ServiceRegistration — no per-request configuration needed here.
    /// </summary>
    public class StripeService : IStripeService
    {
        public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            decimal amount, string orderId)
        {
            var options = new PaymentIntentCreateOptions
            {
                // EGP → Stripe uses smallest currency unit (piastres = 1/100 EGP)
                Amount   = (long)(amount * 100),
                Currency = "egp",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = orderId
                }
            };

            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options);

            return (intent.Id, intent.ClientSecret);
        }
    }
}
