using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;

namespace ShopEgypt.Application.Interfaces.IOrderService
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates an in-memory pending order with the given address and cart items.
        /// Does NOT save to the database — data is kept in memory for session flow.
        /// Returns the in-memory Order object.
        /// </summary>
        Task<Order> CreatePendingOrderAsync(string userId, AddressDTO addressDto);

        /// <summary>
        /// Saves the in-memory pending order (and its items) to the database.
        /// Call this only after payment is confirmed.
        /// </summary>
        Task SavePendingOrderAsync(Order order, AddressDTO addressDto, decimal totalAmount);
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
        /// Saves the pending order to DB, then marks Payment as Succeeded, updates Order to Confirmed,
        /// and clears the cart. Call this only after successful payment.
        /// </summary>
        Task ConfirmPaymentAsync(int orderId);

        /// <summary>
        /// Retrieves all orders for a given user, including their OrderItems and Product navigations.
        /// </summary>
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
    }
}
