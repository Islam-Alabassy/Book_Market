using DataAccess;
using Models;
using Microsoft.AspNetCore.Mvc;

namespace Book_Market.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MainDbContext db;

        public CategoryController(MainDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            List<Category> categories = db.Categories.ToList();
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
                db.Categories.Add(category);
                db.SaveChanges();
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
            Category? category = db.Categories.Find(id);
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
                db.Categories.Update(category);
                db.SaveChanges();
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
            Category? category = db.Categories.Find(id);
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
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction("Index");
        }
    }
}
