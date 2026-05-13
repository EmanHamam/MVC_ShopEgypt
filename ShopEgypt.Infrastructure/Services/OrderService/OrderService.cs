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
            // 1. Create Address object in MEMORY (do NOT persist yet)
            var address = new Address
            {
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                ZipCode = addressDto.ZipCode,
                Country = addressDto.Country,
                AppUserId = userId
            };
            // NOTE: Address is NOT saved to DB yet — will be saved in SavePendingOrderAsync()

            // 2. Create Order header (Pending) in MEMORY (do NOT persist yet)
            var order = new Order
            {
                ApplicationUserId = userId,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0,
                ShippingAddress = address   // ← Local reference only
            };
            // NOTE: Order is NOT saved to DB yet — will be saved in SavePendingOrderAsync()

            // 3. Merge any session cart into the DB cart first.
            //    This handles the case where the user added items while unauthenticated
            //    and then logged in before checking out.
            await _cartService.MergeSessionCartToUserCartAsync();

            // 4. Load cart items from DB
            var cartItems = await _cartService.GetCartAsync();

            // 5. Create OrderItem objects in MEMORY (do NOT persist yet)
            //    GenericRepository.GetAllAsync() has no .Include(), so CartItem.Product
            //    navigation may be null for DB-cart users. Explicitly load the Product
            //    by ProductId to guarantee a non-zero UnitPrice.
            var orderItems = new List<OrderItem>();
            decimal total = 0;
            foreach (var cartItem in cartItems)
            {
                var product = cartItem.Product
                                ?? await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                var unitPrice = product?.Price ?? 0;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice
                    // NOTE: OrderId will be set when order is saved
                };
                orderItems.Add(orderItem);
                total += unitPrice * cartItem.Quantity;
            }

            // 6. Calculate total and assign order items
            order.TotalAmount = total;
            order.OrderItems = orderItems;  // ← Assign items to order

            // NOTE: No SaveAsync() call here — all data is kept in MEMORY.
            //       The order will be persisted to the database only after
            //       payment is confirmed in SavePendingOrderAsync().

            return order;
        }

        /// <summary>
        /// Saves the pending order (and its items) to the database.
        /// Call this only after payment is confirmed.
        /// </summary>
        public async Task SavePendingOrderAsync(Order order, AddressDTO addressDto, decimal totalAmount)
        {
            // 1. Persist shipping address
            var address = new Address
            {
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                ZipCode = addressDto.ZipCode,
                Country = addressDto.Country,
                AppUserId = order.ApplicationUserId
            };
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveAsync(); // Generate address.Id

            // 2. Persist order with address reference
            order.TotalAmount = totalAmount; // Update total amoun
            order.ApplicationUserId = address.AppUserId;
            order.ShippingAddress = address;  // Ensure navigation is set
            order.Id = 0;  // ← Reset Id so EF generates a new one (avoid IDENTITY_INSERT error)
            
            // Store items temporarily before clearing (EF cascade would save them)
            var itemsToSave = order.OrderItems?.ToList() ?? new List<OrderItem>();
            order.OrderItems.Clear();  // ← Clear so EF doesn't auto-save duplicates
            
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync(); // Generate order.Id

            // 3. Persist order items explicitly (only once)
            if (itemsToSave.Any())
            {
                foreach (var item in itemsToSave)
                {
                    item.OrderId = order.Id;
                    item.Id = 0;  // ← Reset Id so EF generates a new one (avoid IDENTITY_INSERT error)
                    await _unitOfWork.OrderItems.AddAsync(item);
                }
                await _unitOfWork.SaveAsync();
            }
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

        public async Task ConfirmPaymentAsync(int orderId, Order order, AddressDTO addressDto)
        {
            // NOTE: Order is already saved to DB by PaymentSuccess before this method is called
            // Here we only update the payment status and clear the cart

            // 1. Update payment row to Succeeded
            var payments = await _unitOfWork.Payments.GetAllAsync();
            var payment = payments.FirstOrDefault(p => p.OrderId == orderId);

            if (payment != null)
            {
                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;
                await _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveAsync();
            }

            // 2. Move order to Processing
            await UpdateOrderStatusAsync(orderId, OrderStatus.Processing);

            // 3. Clear the cart only after successful payment
            await _cartService.ClearCartAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.Where(o => o.ApplicationUserId == userId).ToList();
        }
    }
}
