using DataAccess;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;

namespace Book_Market.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            List<Category> categories = categoryRepo.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //if (category.Name == category.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "The Name can not exactly match the Display Order number");
            //}
            if (ModelState.IsValid)
            {
                categoryRepo.Add(category);
                categoryRepo.Save();
                TempData["success"] = "DATA CREATED SUCCESSFULLY";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if(id== null||id==0)
            {
                return NotFound();
            }
            Category? category = categoryRepo.Get(u=>u.CategoryId==id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            
            if (ModelState.IsValid)
            {
                categoryRepo.Update(category);
                categoryRepo.Save();
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
            Category? category = categoryRepo.Get(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = categoryRepo.Get(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            categoryRepo.Remove(category);
            categoryRepo.Save();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction("Index");
        }
    }
}
