namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class OrderItemDTO
    {
        /// <summary>String representation of int OrderItem.Id.</summary>
        public string OrderItemId { get; set; }

        /// <summary>String representation of int Order.Id.</summary>
        public string OrderId { get; set; }

        /// <summary>The actual product FK — used when inserting the DB row.</summary>
        public int ProductId { get; set; }

        /// <summary>Display name pulled from Product.Title navigation.</summary>
        public string ProductName { get; set; }

        /// <summary>Display description pulled from Product.Description navigation.</summary>
        public string ProductDescription { get; set; }

        public int Quantity { get; set; }

        /// <summary>Decimal to match Product.Price and OrderItem.UnitPrice columns.</summary>
        public decimal UnitPrice { get; set; }
    }
}
