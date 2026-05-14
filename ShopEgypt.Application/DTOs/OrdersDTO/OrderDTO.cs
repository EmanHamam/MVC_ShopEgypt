using ShopEgypt.Domain.Enums;

namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class OrderDTO
    {
        /// <summary>String representation of the int Order.Id.</summary>
        public string OrderId { get; set; }

        public string ApplicationUserId { get; set; }

        public OrderStatus OrderStatus { get; set; }
        
        public DateTime OrderDate { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ProcessingAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        /// <summary>Decimal to match the DB decimal(18,2) column.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Shipping address display data. Populated from TempData or
        /// Address table lookup — not a direct DB-backed field on OrderDTO.
        /// </summary>
        public AddressDTO Address { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
