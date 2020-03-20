using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        public readonly IUnitOfWork _unitOfwork;

        public CoverTypeController(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType covertype = new CoverType();
            if (id == null)
            {
                return View(covertype);
            }
            //covertype = _unitOfwork.CoverType.Get(id.GetValueOrDefault());
            var param = new DynamicParameters();
            param.Add("@Id", id);
            //var objFromDb = _unitOfwork.CoverType.Get(id);
            covertype = _unitOfwork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, param);
            if (covertype == null)
            {
                return NotFound();
            }
            return View(covertype);
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfwork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll,null);
            return Json(new { data = allObj });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType covertype)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", covertype.Name);
                if (covertype.Id == 0)
                {
                    //_unitOfwork.CoverType.Add(covertype);
                    _unitOfwork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                }
                else
                {
                    parameter.Add("@Id", covertype.Id);
                    _unitOfwork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                    //_unitOfwork.CoverType.Update(covertype);
                }
                _unitOfwork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(covertype);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@Id",id);
            //var objFromDb = _unitOfwork.CoverType.Get(id);
            var objFromDb = _unitOfwork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, param);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //_unitOfwork.CoverType.Remove(objFromDb);
            _unitOfwork.SP_Call.Execute(SD.Proc_CoverType_Delete, param);
            _unitOfwork.save();
            return Json(new { success = true, message = "Delete Successful" });
        }


        
        #endregion 
    }
}