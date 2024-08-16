using Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataAccess.Repository.IRepository;

namespace Book_Market.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository productRepo;
        private readonly ICategoryRepository categoryRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            return View();
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
