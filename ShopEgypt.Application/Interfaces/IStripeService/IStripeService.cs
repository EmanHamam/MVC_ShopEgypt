namespace ShopEgypt.Application.Interfaces.IStripeService
{
    public interface IStripeService
    {
        /// <summary>
        /// Creates a Stripe PaymentIntent for the given amount in EGP (piastres conversion
        /// handled internally). Returns the PaymentIntentId and the client_secret needed
        /// by Stripe.js on the frontend.
        /// </summary>
        /// <param name="amount">Order total in EGP (e.g. 150.00 m).</param>
        /// <param name="orderId">String representation of the Order int ID, stored in Stripe metadata.</param>
        Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            decimal amount,
            string orderId);
    }
}
