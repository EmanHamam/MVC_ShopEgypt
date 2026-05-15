using ShopEgypt.Application.DTOs.Admin;
using ShopEgypt.Application.Interfaces.ICustomerService;
using ShopEgypt.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.Services.CustomerService
{
    public class CustomerService(IUnitOfWork _unitOfWork) : ICustomerService
    {
        public async Task<List<AdminCustomerListDTO>> GetAllCustomersForAdminAsync()
        {
            var users = await _unitOfWork.ApplicationUsers.GetAllAsync();
            var orders = await _unitOfWork.Orders.GetAllAsync();

            var customers = users
                .Select(user =>
                {
                    var userOrders = orders.Where(o => o.ApplicationUserId == user.Id).ToList();

                    var fullName =
                        $"{user.FirstName} {user.LastName}".Trim();

                    if (string.IsNullOrWhiteSpace(fullName))
                        fullName = user.UserName ?? user.Email ?? "Unknown Customer";

                    return new AdminCustomerListDTO
                    {
                        UserId = user.Id,
                        FullName = fullName,
                        Email = user.Email ?? string.Empty,
                        OrdersCount = userOrders.Count,
                        TotalSpent = userOrders.Sum(o => o.TotalAmount),
                        MemberSinceYear = user.CreatedAt.HasValue
                            ? user.CreatedAt.Value.Year
                            : DateTime.UtcNow.Year
                    };
                })
                .OrderByDescending(c => c.TotalSpent)
                .ToList();

            return customers;
        }
    }
}
