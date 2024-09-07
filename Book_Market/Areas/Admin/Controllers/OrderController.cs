using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Utilities;

namespace Book_Market.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository orderHeaderRepo;
        private readonly IOrderDetailRepository orderDetailRepo;
        [BindProperty]
        public OrderVM OrderVM {  get; set; }
        public OrderController(IOrderHeaderRepository orderHeaderRepo,IOrderDetailRepository
             orderDetailRepo)
        {
            this.orderHeaderRepo = orderHeaderRepo;
            this.orderDetailRepo = orderDetailRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM() { 
            OrderHeader = orderHeaderRepo.Get(u=>u.Id==id,includeProperties:"ApplicationUser"),
            OrderDetail = orderDetailRepo.GetAll(u=>u.OrderHeaderId==id,includeProperties:"Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =$"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDB = orderHeaderRepo.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDB.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDB.State = OrderVM.OrderHeader.State;
            orderHeaderFromDB.City = OrderVM.OrderHeader.City;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDB.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber)) 
            { 
                orderHeaderFromDB.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            orderHeaderRepo.Update(orderHeaderFromDB);
            orderHeaderRepo.Save();
            TempData["success"] = "Order Details updated successfully";
            return RedirectToAction(nameof(Details), new {id= orderHeaderFromDB.Id});
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult StartProcessing()
        {
            orderHeaderRepo.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusProcessing);
            orderHeaderRepo.Save();
            TempData["success"] = "Order Details updated successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult ShipOrder()
        {
            var orderHeader = orderHeaderRepo.Get(u=>u.Id==OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier= OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            orderHeaderRepo.Update(orderHeader);
            orderHeaderRepo.Save();
            TempData["success"] = "Order Shipped successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public IActionResult CancelOrder() {
            var orderHeader = orderHeaderRepo.Get(u=>u.Id==OrderVM.OrderHeader.Id);
            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                ///Use Stripe Refund

                orderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                orderHeaderRepo.UpdateStatus(orderHeader.Id, SD.StatusCancelled,SD.StatusCancelled);
            }
            orderHeaderRepo.Save();
            TempData["success"] = "Order Cancelld successfully";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_Pay_Now()
        { 

            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string? status)
        {
            List<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = orderHeaderRepo.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = orderHeaderRepo.GetAll(u=>u.ApplicatioUserId == UserId,includeProperties: "ApplicationUser").ToList();
            }
            if (status != null)
            {
                switch (status)
                {
                    case "pending":
                        orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPendeing).ToList();
                        break;
                    case "inprocess":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusProcessing).ToList();

                        break;
                    case "completed":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped).ToList();

                        break;
                    case "approved":
                        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved).ToList();

                        break;
                    default:
                        break;
                }
            }
            return Json(new { data = orderHeaders });
        }


        #endregion
    }
}
