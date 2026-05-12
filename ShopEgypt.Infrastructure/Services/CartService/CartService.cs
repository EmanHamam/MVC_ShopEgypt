using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.DTOs.CartDTO;
using ShopEgypt.Application.Extensions;
using ShopEgypt.Application.Interfaces;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.UnitOfWork;
using System.Linq;
using System.Security.Claims;

namespace ShopEgypt.Infrastructure.Services.CartService
{
    public class CartService(IUnitOfWork _unitOfWork,IHttpContextAccessor _httpContextAccessor,UserManager<ApplicationUser> _userManager) : ICartService
    {
        private const string SessionCartKey = "Cart";
        

        private HttpContext HttpContext => _httpContextAccessor.HttpContext?? throw new InvalidOperationException("No active HttpContext found.");

        private ISession Session => HttpContext.Session;

        private bool IsAuthenticated =>HttpContext.User?.Identity?.IsAuthenticated ?? false;

        private string? UserId =>HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<List<CartItem>> GetCartAsync()
        {
            if (IsAuthenticated)
            {
                return await GetDatabaseCartItemsAsync();
            }

            return await GetSessionCartItemsAsDbLikeModelAsync();
        }

        public async Task AddToCartAsync(int productId, int qty = 1)
        {
            if (qty <= 0) qty = 1;

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found.");

            if (IsAuthenticated)
            {
                await AddToDatabaseCartAsync(productId, qty);
            }
            else
            {
                AddToSessionCart(productId, qty);
            }
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            if (IsAuthenticated)
            {
                await RemoveFromDatabaseCartAsync(productId);
            }
            else
            {
                RemoveFromSessionCart(productId);
            }
        }

        public async Task UpdateQuantityAsync(int productId, int qty)
        {
            if (IsAuthenticated)
            {
                await UpdateDatabaseCartQuantityAsync(productId, qty);
            }
            else
            {
                UpdateSessionCartQuantity(productId, qty);
            }
        }

        public async Task<int> GetCartCountAsync()
        {
            var cart = await GetCartAsync();
            return cart.Sum(x => x.Quantity);
        }

        public async Task<decimal> GetSubtotalAsync()
        {
            var cart = await GetCartAsync();
            return cart.Sum(x => x.Product.Price * x.Quantity);
        }

        public async Task<decimal> GetShippingAsync()
        {
            var subtotal = await GetSubtotalAsync();
            return subtotal > 1500 ? 0 : 80;
        }

        public async Task<decimal> GetTotalAsync()
        {
            return await GetSubtotalAsync() + await GetShippingAsync();
        }

        public async Task ClearCartAsync()
        {
            if (IsAuthenticated)
            {
                var userCart = await GetOrCreateUserCartAsync();
                var items = await GetUserCartItemsInternalAsync(userCart.Id);

                foreach (var item in items)
                {
                    _unitOfWork.CartItems.Delete(item);
                }

                await _unitOfWork.SaveAsync();
            }
            else
            {
                Session.RemoveObject(SessionCartKey);
            }
        }

        public async Task MergeSessionCartToUserCartAsync()
        {
            if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
                return;

            var sessionItems = Session.GetObject<List<SessionCartItem>>(SessionCartKey) ?? new List<SessionCartItem>();
            if (!sessionItems.Any())
                return;

            foreach (var item in sessionItems)
            {
                await AddToDatabaseCartAsync(item.ProductId, item.Qty);
            }

            Session.RemoveObject(SessionCartKey);
        }


        private void AddToSessionCart(int productId, int qty)
        {
            var cart = Session.GetObject<List<SessionCartItem>>(SessionCartKey) ?? new List<SessionCartItem>();

            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Qty += qty;
            }
            else
            {
                cart.Add(new SessionCartItem
                {
                    ProductId = productId,
                    Qty = qty
                });
            }

            Session.SetObject(SessionCartKey, cart);
        }

        private void RemoveFromSessionCart(int productId)
        {
            var cart = Session.GetObject<List<SessionCartItem>>(SessionCartKey) ?? new List<SessionCartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                Session.SetObject(SessionCartKey, cart);
            }
        }

        private void UpdateSessionCartQuantity(int productId, int qty)
        {
            var cart = Session.GetObject<List<SessionCartItem>>(SessionCartKey) ?? new List<SessionCartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
                return;

            if (qty <= 0)
                cart.Remove(item);
            else
                item.Qty = qty;

            Session.SetObject(SessionCartKey, cart);
        }

        private async Task<List<CartItem>> GetSessionCartItemsAsDbLikeModelAsync()
        {
            var sessionItems = Session.GetObject<List<SessionCartItem>>(SessionCartKey) ?? new List<SessionCartItem>();
            var result = new List<CartItem>();

            foreach (var item in sessionItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    continue;

                // Load product images explicitly (GenericRepository.Find doesn't include navigation properties)
                var allImages = await _unitOfWork.ProductImages.GetAllAsync();
                var imagesForProduct = allImages.Where(pi => pi.ProductId == product.Id)
                                                .OrderBy(pi => pi.DisplayOrder)
                                                .ToList();
                product.ProductImages = imagesForProduct;

                result.Add(new CartItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Qty,
                    //UnitPrice = product.Price,
                    Product = product
                });
            }

            return result;
        }

        private async Task AddToDatabaseCartAsync(int productId, int qty)
        {
            var userCart = await GetOrCreateUserCartAsync();
            var items = await GetUserCartItemsInternalAsync(userCart.Id);

            var existingItem = items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += qty;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                await _unitOfWork.CartItems.AddAsync(new CartItem
                {
                    CartId = userCart.Id,
                    ProductId = productId,
                    Quantity = qty
                });
            }

            await _unitOfWork.SaveAsync();
        }

        private async Task RemoveFromDatabaseCartAsync(int productId)
        {
            var userCart = await GetOrCreateUserCartAsync();
            var items = await GetUserCartItemsInternalAsync(userCart.Id);
            var item = items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
                return;

            _unitOfWork.CartItems.Delete(item);
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateDatabaseCartQuantityAsync(int productId, int qty)
        {
            var userCart = await GetOrCreateUserCartAsync();
            var items = await GetUserCartItemsInternalAsync(userCart.Id);
            var item = items.FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
                return;

            if (qty <= 0)
            {
                _unitOfWork.CartItems.Delete(item);
            }
            else
            {
                item.Quantity = qty;
                _unitOfWork.CartItems.Update(item);
            }

            await _unitOfWork.SaveAsync();
        }

        private async Task<Cart> GetOrCreateUserCartAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                throw new Exception("User is not authenticated.");

            var carts = await _unitOfWork.Carts.GetAllAsync();
            var userCart = carts.FirstOrDefault(x => x.ApplicationUserId == UserId);

            if (userCart != null)
                return userCart;

            userCart = new Cart
            {
                ApplicationUserId = UserId
            };

            await _unitOfWork.Carts.AddAsync(userCart);
            await _unitOfWork.SaveAsync();

            return userCart;
        }

        private async Task<List<CartItem>> GetDatabaseCartItemsAsync()
        {
            var userCart = await GetOrCreateUserCartAsync();
            var items    = await GetUserCartItemsInternalAsync(userCart.Id);

            foreach (var item in items)
            {
                if (item.Product == null)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        var allImages = await _unitOfWork.ProductImages.GetAllAsync();
                        product.ProductImages = allImages.Where(pi => pi.ProductId == product.Id)
                                                         .OrderBy(pi => pi.DisplayOrder)
                                                         .ToList();
                    }
                    item.Product = product;
                }
                
                
            }

            return items;
        }

        private async Task<List<CartItem>> GetUserCartItemsInternalAsync(int cartId)
        {
            var allItems = await _unitOfWork.CartItems.GetAllAsync();
            return allItems
                .Where(x => x.CartId == cartId)
                .ToList();
        }
    }
}