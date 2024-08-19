using DataAccess;
using Models;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Utilities;

namespace Book_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository CompanyRepo;
       
        public CompanyController(ICompanyRepository CompanyRepo)
        {
            this.CompanyRepo = CompanyRepo;
            
        }
        public IActionResult Index()
        {
            List<Company> Companies = CompanyRepo.GetAll().ToList();
           
            return View(Companies);
        }
        public IActionResult Create()
        {
           
            return View(model: new Company());
        }
        [HttpPost]
        public IActionResult Create(Company Company,IFormFile? fileUploaded)
        {
            //if (Company.Name == Company.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "The Name can not exactly match the Display Order number");
            //}
            if (ModelState.IsValid)
            {
                CompanyRepo.Add(Company);
                CompanyRepo.Save();
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
            Company? Company = CompanyRepo.Get(u => u.CompanyId == id);
            if (Company == null)
            {
                return NotFound();
            }
            return View(Company);
        }
        [HttpPost]
        public IActionResult Edit(Company Company, IFormFile? fileUploaded)
        {

            if (ModelState.IsValid)
            {              
                CompanyRepo.Update(Company);
                CompanyRepo.Save();
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
            Company? Company = CompanyRepo.Get(u => u.CompanyId == id);
            if (Company == null)
            {
                return NotFound();
            }
            return View(Company);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? Company = CompanyRepo.Get(u => u.CompanyId == id);
            if (Company == null)
            {
                return NotFound();
            }
            CompanyRepo.Remove(Company);
            CompanyRepo.Save();
            TempData["success"] = "DATA DELETED SUCCESSFULLY";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = CompanyRepo.GetAll().ToList();
            return Json(new {data =  Companies});
        }

        [HttpDelete]
        public IActionResult DeleteApi(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? Company = CompanyRepo.Get(u => u.CompanyId == id);
            
            if (Company == null)
            {
                return NotFound();
            }
           
            CompanyRepo.Remove(Company);
            CompanyRepo.Save();
            return Json(new { success = true,message="Delete successsful" });
        }
        #endregion
    }
}
