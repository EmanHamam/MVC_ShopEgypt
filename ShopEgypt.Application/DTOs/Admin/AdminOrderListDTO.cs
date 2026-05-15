using ShopEgypt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminOrderListDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber => $"SE-{OrderId}";
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
