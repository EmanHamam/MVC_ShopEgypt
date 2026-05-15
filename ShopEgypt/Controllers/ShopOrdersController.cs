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
    [Route("Orders/[action]/{orderId?}")]
    public class ShopOrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService                 _cartService;
        private readonly IOrderService                _orderService;
        private readonly IStripeService               _stripeService;
        private readonly IConfiguration               _config;
        private readonly IAddressService              _addressService;  

        public ShopOrdersController(
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

        public async Task<IActionResult> MyOrders(int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId!);

            int pageSize = 10;
            var totalOrders = orders.Count;
            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            
            var paginatedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var orderDtos = new List<OrderDTO>();
            foreach(var order in paginatedOrders)
            {
                orderDtos.Add(await _orderService.BuildOrderDTOAsync(order));
            }

            var viewModel = new OrderListViewModel
            {
                Orders = orderDtos,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalOrders = totalOrders
            };

            return View(viewModel);
        }

        // ════════════════════════════════════════════════════
        // View Details of specific order
        // ════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Detail(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (order.ApplicationUserId != userId)
                return Forbid();

            var orderDto = await _orderService.BuildOrderDTOAsync(order);

            var viewModel = new OrderDetailViewModel
            {
                Order = orderDto
            };

            return View(viewModel);
        }

        // ════════════════════════════════════════════════════
        // Cancel Order
        // ════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (order.ApplicationUserId != userId)
                return Forbid();

            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Confirmed)
            {
                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Order cannot be cancelled at this stage.";
            }

            return RedirectToAction(nameof(Detail), new { orderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayPendingOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (order.ApplicationUserId != userId)
                return Forbid();

            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "Only pending orders can be paid.";
                return RedirectToAction(nameof(Detail), new { orderId = orderId });
            }

            decimal grandTotal = order.TotalAmount;

            // Create Stripe PaymentIntent using the REAL order.Id
            var (paymentIntentId, clientSecret) =
                await _stripeService.CreatePaymentIntentAsync(grandTotal, order.Id.ToString());

            // Pass payment data to Payment view via TempData
            TempData["OrderId"]        = order.Id.ToString();
            TempData["PaymentIntentId"] = paymentIntentId;
            TempData["ClientSecret"]   = clientSecret;
            TempData["PublishableKey"] = _config["Stripe:PublishableKey"];
            TempData["TotalAmount"]    = grandTotal.ToString("F2");

            return RedirectToAction(nameof(Payment));
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

            var orderDto = await _orderService.BuildOrderDTOAsync(order, addressDto);
            
            // Override ID for pending orders to avoid confusing the view
            if (order.Id == 0) orderDto.OrderId = "pending";

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

            // NOW save the order to the database FIRST
            await _orderService.SavePendingOrderAsync(order, addressDto, grandTotal);

            // Create Stripe PaymentIntent using the REAL order.Id
            var (paymentIntentId, clientSecret) =
                await _stripeService.CreatePaymentIntentAsync(grandTotal, order.Id.ToString());

            // Pass payment data to Payment view via TempData
            TempData["OrderId"]        = order.Id.ToString();
            TempData["PaymentIntentId"] = paymentIntentId;
            TempData["ClientSecret"]   = clientSecret;
            TempData["PublishableKey"] = _config["Stripe:PublishableKey"];
            TempData["TotalAmount"]    = grandTotal.ToString("F2");

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
            TempData.Keep("OrderId");
            TempData.Keep("PaymentIntentId");
            TempData.Keep("TotalAmount");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentSuccess(string orderId)
        {
            if (!int.TryParse(orderId, out int orderIdInt))
                return RedirectToAction(nameof(Address));

            var order = await _orderService.GetOrderByIdAsync(orderIdInt);
            if (order == null)
                return RedirectToAction(nameof(Address));

            string paymentIntentId = TempData["PaymentIntentId"] as string ?? "";
            var grandTotal = order.TotalAmount;

            await _orderService.AttachPaymentIntentAsync(order.Id, paymentIntentId, grandTotal);

            // Mark payment as succeeded and order as Confirmed, clear cart
            await _orderService.ConfirmPaymentAsync(order.Id);

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

            decimal subtotal = orderDto.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
            decimal shippingFee = orderDto.TotalAmount - subtotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.GrandTotal  = orderDto.TotalAmount;

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
