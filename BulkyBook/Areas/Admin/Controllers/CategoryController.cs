using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public readonly IUnitOfWork _unitOfwork;

        public CategoryController(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = _unitOfwork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfwork.Category.Add(category);
                }
                else
                {
                    _unitOfwork.Category.Update(category);
                }
                _unitOfwork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfwork.Category.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfwork.Category.Remove(objFromDb);
            _unitOfwork.save();
            return Json(new { success = true, message = "Delete Successful" });
        }


        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfwork.Category.GetAll();
            return Json(new { data = allObj });
        }
        #endregion 
    }
}