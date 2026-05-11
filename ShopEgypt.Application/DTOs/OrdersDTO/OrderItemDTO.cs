using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class OrderItemDTO
    {

        public String OrderItemId { get; set; }

        public String OrderId { get; set; }
        public String ProductName { get; set; }

        public string ProductDescription { get; set; }


        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

    }
}
