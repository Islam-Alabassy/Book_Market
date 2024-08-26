using Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataAccess.Repository.IRepository;
using Models.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Utilities;

namespace Book_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository productRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IShoppingCartRepository shopCartRepo;
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository categoryRepo, IShoppingCartRepository shopCartRepo)
        {
            _logger = logger;
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
            this.shopCartRepo = shopCartRepo;

        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = productRepo.GetAll(includeProperties:"Category");
            return View(products);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
               Product = productRepo.Get(u => u.ProductId == id, includeProperties: "Category"),
               ProductId = id,
               Count = 1
            };
           
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            
            ShoppingCart cartFrmDb = shopCartRepo.Get(u=>u.ApplicationUserId == userId
            && u.ProductId==shoppingCart.ProductId);
            if(cartFrmDb == null)
            {
                //Add cart record
                shopCartRepo.Add(shoppingCart);
                shopCartRepo.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    shopCartRepo.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            else
            {
                //ShoppingCart exists
                cartFrmDb.Count += shoppingCart.Count;
                shopCartRepo.Update(cartFrmDb);
                shopCartRepo.Save();
            }
            
            
            TempData["success"] = "Cart Updated Successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
