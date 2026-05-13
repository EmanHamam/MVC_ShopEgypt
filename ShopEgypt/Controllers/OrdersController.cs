using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Application.Interfaces.IAddressService;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Interfaces.IOrderService;
using ShopEgypt.Application.Interfaces.IStripeService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;
using System.Text.Json;

namespace ShopEgypt.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService                 _cartService;
        private readonly IOrderService                _orderService;
        private readonly IStripeService               _stripeService;
        private readonly IConfiguration               _config;
        private readonly IAddressService              _addressService;  

        public OrdersController(
            UserManager<ApplicationUser> userManager,
            ICartService                 cartService,
            IOrderService                orderService,
            IStripeService               stripeService,
            IConfiguration               config,
            IAddressService              addressService)
        {
            _userManager   = userManager;
            _cartService   = cartService;
            _orderService  = orderService;
            _stripeService = stripeService;
            _config        = config;
            _addressService = addressService;
        }
        // ════════════════════════════════════════════════════
        // View All order to specific user
        // ════════════════════════════════════════════════════

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId!);
            return View(orders);
        }

        // ════════════════════════════════════════════════════
        // STEP 1 — ADDRESS
        // ════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Address()
        {
            var userId = _userManager.GetUserId(User);

            // Pre-fill form with latest saved address for this user (if any)
            var prefilled = await _addressService.TryGetSavedAddressAsync(userId!);
            return View(prefilled ?? new AddressDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Address(AddressDTO addressDto)
        {
            if (!ModelState.IsValid)
                return View(addressDto);

            var userId = _userManager.GetUserId(User)!;

            // Create Order (Pending) in MEMORY + Address + OrderItems
            // NOTE: Nothing is saved to the database yet — data is kept in TempData
            var order = await _orderService.CreatePendingOrderAsync(userId, addressDto);

            // Persist data in TempData for the next steps
            TempData["OrderId"]  = Guid.NewGuid().ToString();  // Temporary session ID
            TempData["Address"]  = JsonSerializer.Serialize(addressDto);
            TempData["Order"]    = JsonSerializer.Serialize(order);
            return RedirectToAction(nameof(ViewSummary));
        }

        // ════════════════════════════════════════════════════
        // STEP 2 — VIEW SUMMARY
        // ════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> ViewSummary()
        {
            if (!TryGetTempOrder(out Order? order) || order == null)
                return RedirectToAction(nameof(Address));

            var addressDto = TryDeserializeAddress();
            if (addressDto == null)
                return RedirectToAction(nameof(Address));

            // Build OrderDTO from the in-memory order
            var orderDto = new OrderDTO
            {
                OrderId = "pending",  // Not yet saved to DB
                ApplicationUserId = order.ApplicationUserId,
                OrderStatus = order.Status,
                TotalAmount = order.TotalAmount,
                Address = addressDto,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = "pending",
                    OrderId = "pending",
                    ProductId = oi.ProductId,
                    ProductName = "Loading...",
                    ProductDescription = string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemDTO>()
            };

            // Keep TempData alive for the next step (ConfirmOrder POST)
            TempData.Keep("OrderId");
            TempData.Keep("Address");
            TempData.Keep("Order");
            return View(orderDto);
        }

        // ════════════════════════════════════════════════════
        // STEP 3 — CONFIRM ORDER
        // ════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(string orderId)
        {
            if (!TryGetTempOrder(out Order? order) || order == null)
                return RedirectToAction(nameof(Address));

            var addressDto = TryDeserializeAddress();
            if (addressDto == null)
                return RedirectToAction(nameof(Address));

            // Calculate shipping from the pending order subtotal
            decimal shippingFee = order.TotalAmount > 1500 ? 0 : 80;
            decimal grandTotal  = order.TotalAmount + shippingFee;

            // Create Stripe PaymentIntent BEFORE saving order to DB
            // Use a temporary session identifier for payment intent metadata
            string tempOrderRef = $"temp_{DateTime.UtcNow.Ticks}";
            var (paymentIntentId, clientSecret) =
                await _stripeService.CreatePaymentIntentAsync(grandTotal, tempOrderRef);

            // Pass payment data to Payment view via TempData
            TempData["OrderId"]        = tempOrderRef;
            TempData["PaymentIntentId"] = paymentIntentId;
            TempData["ClientSecret"]   = clientSecret;
            TempData["PublishableKey"] = _config["Stripe:PublishableKey"];
            TempData["TotalAmount"]    = grandTotal.ToString("F2");
            TempData["ShippingFee"]    = shippingFee.ToString("F2");

            // Keep Order and Address for final save in PaymentSuccess
            TempData.Keep("Order");
            TempData.Keep("Address");

            return RedirectToAction(nameof(Payment));
        }

        // ════════════════════════════════════════════════════
        // STEP 4 — STRIPE PAYMENT
        // ════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult Payment()
        {
            var clientSecret   = TempData["ClientSecret"]   as string;
            var publishableKey = TempData["PublishableKey"] as string;
            var totalAmount    = TempData["TotalAmount"]    as string;
            var orderId        = TempData["OrderId"]        as string;

            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(publishableKey))
                return RedirectToAction(nameof(Address));

            ViewBag.ClientSecret   = clientSecret;
            ViewBag.PublishableKey = publishableKey;
            ViewBag.TotalAmount    = totalAmount;
            ViewBag.OrderId        = orderId;

            // Keep data for PaymentSuccess
            TempData.Keep("Order");
            TempData.Keep("Address");
            TempData.Keep("PaymentIntentId");
            TempData.Keep("ShippingFee");
            TempData.Keep("TotalAmount");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentSuccess(string orderId)
        {
            // Deserialize the pending order and address from TempData
            if (!TryGetTempOrder(out Order? order) || order == null)
                return RedirectToAction(nameof(Address));

            var addressDto = TryDeserializeAddress();
            if (addressDto == null)
                return RedirectToAction(nameof(Address));

            string paymentIntentId = TempData["PaymentIntentId"] as string ?? "";
            var totalAmountStr = TempData["TotalAmount"] as string ?? "0";
            // NOW save the order to the database for the first time
            await _orderService.SavePendingOrderAsync(order, addressDto , decimal.Parse(totalAmountStr));

            // Create Payment record after order is saved (now we have order.Id)
            //string? shippingFeeStr = TempData["ShippingFee"] as string;
            //decimal shippingFee = string.IsNullOrEmpty(shippingFeeStr) ? 0 : decimal.Parse(shippingFeeStr);
            //decimal grandTotal = order.TotalAmount + shippingFee;
            
            var grandTotal = order.TotalAmount;

            await _orderService.AttachPaymentIntentAsync(order.Id, paymentIntentId, grandTotal);

            // Mark payment as succeeded and order as Processing, clear cart
            await _orderService.ConfirmPaymentAsync(order.Id, order, addressDto);

            return RedirectToAction(nameof(OrderConfirmation), new { orderId = order.Id });
        }

        // ════════════════════════════════════════════════════
        // STEP 5 — ORDER CONFIRMATION
        // ════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return RedirectToAction(nameof(Address));

            var orderDto = await _orderService.BuildOrderDTOAsync(order);

            decimal shippingFee = orderDto.TotalAmount > 1500 ? 0 : 80;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.GrandTotal  = orderDto.TotalAmount + shippingFee;

            return View(orderDto);
        }

        // ════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ════════════════════════════════════════════════════

        private bool TryGetTempOrderId(out int orderId)
        {
            orderId = 0;
            var raw = TempData["OrderId"] as string;
            return !string.IsNullOrEmpty(raw) && int.TryParse(raw, out orderId);
        }

        private bool TryGetTempOrder(out Order? order)
        {
            order = null;
            var json = TempData["Order"] as string;
            if (string.IsNullOrEmpty(json)) return false;
            try 
            { 
                order = JsonSerializer.Deserialize<Order>(json);
                return order != null;
            }
            catch { return false; }
        }

        private AddressDTO? TryDeserializeAddress()
        {
            var json = TempData["Address"] as string;
            if (string.IsNullOrEmpty(json)) return null;
            try { return JsonSerializer.Deserialize<AddressDTO>(json); }
            catch { return null; }
        }

        //private async Task<AddressDTO?> TryGetSavedAddressAsync(string userId)
        //{
        //    var cartItems = await _cartService.GetCartAsync();
        //    // Use cart just to confirm user is legit; load address separately via OrderService
        //    // We borrow the address load logic from OrderService's BuildOrderDTOAsync
        //    // by calling GetOrderByIdAsync with a dummy and returning null if nothing found
        //    // — actually just do a direct check in the controller for simplicity:
        //    return null; // OrderService will load it when building OrderDTO
        //}
    }
}
