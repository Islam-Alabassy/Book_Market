using DataAccess;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace Book_Market.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IProductRepository productRepo,ICategoryRepository categoryRepo, IWebHostEnvironment webHostEnvironment)
        {
            this.productRepo = productRepo;
            this.categoryRepo = categoryRepo;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = productRepo.GetAll(includeProperties:"Category").ToList();
           
            return View(products);
        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> categoryList = categoryRepo.GetAll().Select(
               u => new SelectListItem { Text = u.Name, Value = u.CategoryId.ToString() });
            ViewBag.CategoryList = categoryList;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product Product,IFormFile? fileUploaded)
        {
            //if (Product.Name == Product.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "The Name can not exactly match the Display Order number");
            //}
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if(fileUploaded != null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(fileUploaded.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    using(var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        fileUploaded.CopyTo(fileStream);
                    }
                    Product.ImageUrl = @"\images\product\" + fileName;
                }

                productRepo.Add(Product);
                productRepo.Save();
                TempData["success"] = "DATA CREATED SUCCESSFULLY";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = productRepo.Get(u => u.ProductId == id);
            if (Product == null)
            {
                return NotFound();
            }
            IEnumerable<SelectListItem> categoryList = categoryRepo.GetAll().Select(
              u => new SelectListItem { Text = u.Name, Value = u.CategoryId.ToString() });
            ViewBag.CategoryList = categoryList;
            return View(Product);
        }
        [HttpPost]
        public IActionResult Edit(Product Product, IFormFile? fileUploaded)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (fileUploaded != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUploaded.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(Product.ImageUrl))
                    {
                        var oldPath = Path.Combine(wwwRootPath, Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        fileUploaded.CopyTo(fileStream);
                    }
                    Product.ImageUrl = @"\images\product\" + fileName;
                }
               
                productRepo.Update(Product);
                productRepo.Save();
                TempData["success"] = "DATA UPDATED SUCCESSFULLY";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = productRepo.Get(u => u.ProductId == id);
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Product = productRepo.Get(u => u.ProductId == id);
            if (Product == null)
            {
                return NotFound();
            }
            productRepo.Remove(Product);
            productRepo.Save();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction("Index");
        }
    }
}
