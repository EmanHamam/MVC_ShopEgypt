using System.Collections.Generic;

namespace ShopEgypt.Application.DTOs.OrdersDTO
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalOrders { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
