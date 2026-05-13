using Microsoft.AspNetCore.Identity;
using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Application.Interfaces.IAddressService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.UnitOfWork;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.Services.AddressService
{
    public class AddressService() : IAddressService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork) : this()
        { 
            _userManager = userManager; 
            _unitOfWork = unitOfWork;
        }


        public async Task<AddressDTO> TryGetSavedAddressAsync(string userId)
        {
            

            var address = await _unitOfWork.Addresses.GetByUserIdAsync(userId);
            if (address == null) return null;
            return new AddressDTO
            {
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                Country = address.Country
            };

        }

        public Task TrySaveAddAddressAsync(string userId, AddressDTO AddressDTO)
        {
            var address = new Address
            {
                AppUserId = userId,
                Street = AddressDTO.Street,
                City = AddressDTO.City,
                State = AddressDTO.State,
                ZipCode = AddressDTO.ZipCode,
                Country = AddressDTO.Country
            };
            return _unitOfWork.Addresses.AddAsync(address);
        }

        public async Task<AddressDTO> GetFirstAddressAsync(string userId)
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync();
            var saved = addresses
                .Where(a => a.AppUserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();
            if (saved != null)
            {
                return new AddressDTO
                {
                    Street = saved.Street,
                    City = saved.City,
                    State = saved.State,
                    ZipCode = saved.ZipCode,
                    Country = saved.Country
                };
            }
            return null;
        } 
    }
}
