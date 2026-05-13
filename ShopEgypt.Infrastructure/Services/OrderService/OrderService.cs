using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Application.Interfaces.IAddressService;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Interfaces.IOrderService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;
using ShopEgypt.Infrastructure.UnitOfWork;

namespace ShopEgypt.Infrastructure.Services.OrderService
{
    public class OrderService(IUnitOfWork _unitOfWork, ICartService _cartService , IAddressService _addressService) : IOrderService
    {
        // ──────────────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ──────────────────────────────────────────────────────────────────────────

        public async Task<Order> CreatePendingOrderAsync(string userId, AddressDTO addressDto)
        {
            // 1. Persist shipping address — let IDENTITY generate the Id
            var address = new Address
            {
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                ZipCode = addressDto.ZipCode,
                Country = addressDto.Country,
                AppUserId = userId
            };
            await _addressService.TrySaveAddAddressAsync(userId, addressDto);
            await _unitOfWork.SaveAsync(); // address.Id is now set by EF

            // 2. Create Order header (Pending) — let IDENTITY generate the Id.
            //    Assign ShippingAddress navigation so EF populates the shadow
            //    ShippingAddressId FK column (created by the existing migration).
            var order = new Order
            {
                ApplicationUserId = userId,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0,
                ShippingAddress = address   // ← EF sets ShippingAddressId automatically
            };
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync(); // order.Id is now set by EF

            // 3. Merge any session cart into the DB cart first.
            //    This handles the case where the user added items while unauthenticated
            //    and then logged in before checking out.
            await _cartService.MergeSessionCartToUserCartAsync();

            // 4. Load cart items from DB
            var cartItems = await _cartService.GetCartAsync();

            // 5. Insert OrderItem rows.
            //    GenericRepository.GetAllAsync() has no .Include(), so CartItem.Product
            //    navigation may be null for DB-cart users. Explicitly load the Product
            //    by ProductId to guarantee a non-zero UnitPrice.
            decimal total = 0;
            foreach (var cartItem in cartItems)
            {
                var product = cartItem.Product
                                ?? await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                var unitPrice = product?.Price ?? 0;

                // Let IDENTITY generate OrderItem.Id — only set FK OrderId from the saved order
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice
                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);
                total += unitPrice * cartItem.Quantity;
            }

            // 6. Update total and save all order items
            order.TotalAmount = total;
            await _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveAsync();

            // NOTE: Cart is NOT cleared here — it is cleared only after
            //       successful payment in ConfirmPaymentAsync.

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.FirstOrDefault(o => o.Id == orderId);
        }

        public async Task<OrderDTO> BuildOrderDTOAsync(Order order, AddressDTO? addressDto = null)
        {
            // Load order items with product navigation
            var allItems = await _unitOfWork.OrderItems.GetAllAsync();
            var orderItems = allItems.Where(i => i.OrderId == order.Id).ToList();

            // Build item DTOs.
            // OrderItems loaded via GetAllAsync() have no .Include(x => x.Product),
            // so always explicitly fetch the Product to get Title, Description, and Price.
            var itemDtos = new List<OrderItemDTO>();
            foreach (var item in orderItems)
            {
                var product = item.Product
                              ?? await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                itemDtos.Add(new OrderItemDTO
                {
                    OrderItemId = item.Id.ToString(),
                    OrderId = order.Id.ToString(),
                    ProductId = item.ProductId,
                    ProductName = product?.Title ?? $"Product #{item.ProductId}",
                    ProductDescription = product?.Description ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice   // already persisted correctly from CreatePendingOrderAsync
                });
            }

            // If no addressDto passed, try to load from Address table by AppUserId
            if (addressDto == null && !string.IsNullOrEmpty(order.ApplicationUserId))
            {
                addressDto = await _addressService.GetFirstAddressAsync(order.ApplicationUserId);
            }

            return new OrderDTO
            {
                OrderId = order.Id.ToString(),
                ApplicationUserId = order.ApplicationUserId,
                OrderStatus = order.Status,
                TotalAmount = order.TotalAmount,
                Address = addressDto ?? new AddressDTO(),
                OrderItems = itemDtos
            };
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order == null) return;

            order.Status = status;
            await _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveAsync();
        }

        public async Task<Payment> AttachPaymentIntentAsync(
            int orderId, string paymentIntentId, decimal amount)
        {
            // Let IDENTITY generate Payment.Id
            var payment = new Payment
            {
                OrderId = orderId,
                StripePaymentIntentId = paymentIntentId,
                Amount = amount,
                Status = PaymentStatus.Pending
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            return payment;
        }

        public async Task ConfirmPaymentAsync(int orderId)
        {
            // Update payment row
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var payment = payments.FirstOrDefault(p => p.OrderId == orderId);

            if (payment != null)
            {
                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;
                await _unitOfWork.Payments.Update(payment);
            }

            // Move order to Processing
            await UpdateOrderStatusAsync(orderId, OrderStatus.Processing);

            // Clear the cart only after successful payment
            await _cartService.ClearCartAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.Where(o => o.ApplicationUserId == userId).ToList();
        }
    }
}
