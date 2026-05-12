using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;

namespace ShopEgypt.Application.Interfaces.IOrderService
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates an Address row, an Order row (Status=Pending), OrderItem rows from the
        /// current cart, updates TotalAmount, and clears the cart. Returns the saved Order.
        /// </summary>
        Task<Order> CreatePendingOrderAsync(string userId, AddressDTO addressDto);

        /// <summary>
        /// Loads an Order entity by its int ID, including OrderItems and Product navigations.
        /// </summary>
        Task<Order?> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Maps a loaded Order entity to an OrderDTO. Pass addressDto from TempData to
        /// populate the display address (avoids needing a FK column on Order).
        /// </summary>
        Task<OrderDTO> BuildOrderDTOAsync(Order order, AddressDTO? addressDto = null);

        /// <summary>
        /// Updates Order.Status and persists to DB.
        /// </summary>
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);

        /// <summary>
        /// Creates a Payment row (Status=Pending) linked to the given order.
        /// </summary>
        Task<Payment> AttachPaymentIntentAsync(int orderId, string paymentIntentId, decimal amount);

        /// <summary>
        /// Sets Payment.Status = Succeeded, PaidAt = UtcNow, Order.Status = Processing.
        /// </summary>
        Task ConfirmPaymentAsync(int orderId);
    }
}
