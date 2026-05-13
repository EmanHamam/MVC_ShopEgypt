using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.IAddressService
{
    public interface IAddressService
    {
        public Task<AddressDTO> TryGetSavedAddressAsync(string userId);
        public Task TrySaveAddAddressAsync(string userId, AddressDTO AddressDTO);
    }
}
