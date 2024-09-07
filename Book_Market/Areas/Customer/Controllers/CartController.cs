using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using Utilities;

namespace Book_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepository shoppingCartRepo;
        private readonly IOrderHeaderRepository orderHeaderRepo;
        private readonly IOrderDetailRepository orderDetailRepo;
        private readonly IApplicationUSerRepository applicationUserRepo;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IShoppingCartRepository shoppingCartRepo,
            IOrderHeaderRepository orderHeaderRepo,
            IOrderDetailRepository orderDetailRepo,
            IApplicationUSerRepository applicationUserRepo)
        {
            this.shoppingCartRepo = shoppingCartRepo;
            this.orderHeaderRepo = orderHeaderRepo;
            this.orderDetailRepo = orderDetailRepo;
            this.applicationUserRepo = applicationUserRepo;
        }
      
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                shoppingCartList = shoppingCartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach(var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        
        public IActionResult Plus(int cartId)
        {
            var cart = shoppingCartRepo.Get(u=>u.ShoppingCartId == cartId);
            cart.Count++;
            shoppingCartRepo.Update(cart);
            shoppingCartRepo.Save();
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Minus(int cartId)
        {
            var cart = shoppingCartRepo.Get(u => u.ShoppingCartId == cartId);
            if (cart.Count <= 1)
            {
                //remove that cart
                shoppingCartRepo.Remove(cart);
                HttpContext.Session.SetInt32(SD.SessionCart,
                    shoppingCartRepo.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count()-1);
            }
            else
            {
                cart.Count--;
                shoppingCartRepo.Update(cart);
            }
            shoppingCartRepo.Save();
            return RedirectToAction(nameof(Index));
        }
       
        public IActionResult Remove(int cartId)
        {
            var cart = shoppingCartRepo.Get(u => u.ShoppingCartId == cartId);
            
            shoppingCartRepo.Remove(cart);
            HttpContext.Session.SetInt32(SD.SessionCart,
                   shoppingCartRepo.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
            shoppingCartRepo.Save();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                shoppingCartList = shoppingCartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUserRepo.Get(u=>u.Id== userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM); 
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.shoppingCartList = shoppingCartRepo.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
               
            ApplicationUser applicationUser = applicationUserRepo.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicatioUserId = userId;

           
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if(applicationUser.CompanyId.GetValueOrDefault()==0)
            {
                //it is a regular customer accountant.
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPendeing;
            }
            else
            {
                //it is a company user
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }
            orderHeaderRepo.Add(ShoppingCartVM.OrderHeader);
            orderHeaderRepo.Save();

            foreach(var cart in ShoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                { OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                  Price = cart.Price,
                  Count = cart.Count
                  ,ProductId = cart.ProductId,
                  Product = null,
                  OrderHeader = null
                };
                orderDetailRepo.Add(orderDetail);
                
                orderDetailRepo.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer accountant and we need to capture payment.
                //logic strip
            }
            return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.OrderHeader.Id});
        }
       
        public IActionResult OrderConfirmation(int id)
        {
            HttpContext.Session.Clear();
            return View(id);
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if(shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
            
        }
    }
}
