using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class OrderDTO
    {
        
        public string OrderId { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }
        public string ApplicationUserId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public double TotalAmount { get; set; }

        public AddressDTO Address { get; set; }
    }
}
