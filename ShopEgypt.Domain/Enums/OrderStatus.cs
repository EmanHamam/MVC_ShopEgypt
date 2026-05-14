using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled,

    }
}
