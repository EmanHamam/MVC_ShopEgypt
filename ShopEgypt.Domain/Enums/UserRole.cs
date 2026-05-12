using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Domain.Enums
{
    [Flags]
    public enum UserRole
    {
        Admin = 1,
        Customer = 2,
    }
}
