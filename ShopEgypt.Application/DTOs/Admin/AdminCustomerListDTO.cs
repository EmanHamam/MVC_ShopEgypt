using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminCustomerListDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public int MemberSinceYear { get; set; }
        public string Initial =>
            !string.IsNullOrWhiteSpace(FullName)
                ? FullName.Trim()[0].ToString().ToUpper()
                : "?";
    }
}
