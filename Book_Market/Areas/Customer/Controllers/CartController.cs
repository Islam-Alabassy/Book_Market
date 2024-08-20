using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;

namespace Book_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepository shoppingCartRepo;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IShoppingCartRepository shoppingCartRepo)
        {
            this.shoppingCartRepo = shoppingCartRepo;
        }
      
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                shoppingCartList = shoppingCartRepo.GetAll(u => u.ApplicationUserId == userId,includeProperties:"Product")
            };

            foreach(var cart in ShoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
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
            shoppingCartRepo.Save();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {

            return View(); 
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
