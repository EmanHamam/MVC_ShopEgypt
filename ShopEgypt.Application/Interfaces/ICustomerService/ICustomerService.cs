using ShopEgypt.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.ICustomerService
{
    public interface ICustomerService
    {
        Task<List<AdminCustomerListDTO>> GetAllCustomersForAdminAsync();
    }
}
