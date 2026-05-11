using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShopEgypt.Application.DTOs.OrdersDTO;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums;

namespace ShopEgypt.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Address()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Address(AddressDTO addressDTO)
        {
            if (ModelState.IsValid)
            {
                string orderId = Guid.NewGuid().ToString();
                var OrderObject = new OrderDTO
                {
                    OrderId = orderId,
                    ApplicationUserId = _userManager.GetUserAsync(User).Result.Id,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = new List<OrderItemDTO>
                    {
                        new OrderItemDTO
                        {
                            OrderId = orderId,
                            OrderItemId = Guid.NewGuid().ToString(), 
                            ProductName = "Sample Product",
                            ProductDescription = "This is a sample product.",
                            UnitPrice = 100,
                            Quantity = 1
                        },
                        new OrderItemDTO
                        {
                            OrderId = orderId,
                            OrderItemId = Guid.NewGuid().ToString(),
                            ProductName = "Sample Product 2",
                            ProductDescription = "This is a sample product.",
                            UnitPrice = 150,
                            Quantity = 2
                        }
                    },
                    Address = addressDTO
                };
                OrderObject.TotalAmount = OrderObject.OrderItems.Sum(item => item.UnitPrice * item.Quantity);
                return View("ViewSummary", OrderObject);
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        public ActionResult ViewSummary(OrderDTO orderDTO)
        {
            return View(orderDTO);
        }
        [HttpPost]
        public ActionResult ConfirmOrder(OrderDTO orderDTO)
        {
            
            return View("OrderConfirmation", orderDTO);
        }
        [HttpGet]
        public ActionResult OrderConfirmation(OrderDTO orderDTO)
        {
            return View(orderDTO);
        }
    }
}   
