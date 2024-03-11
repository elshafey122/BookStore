using Ecommerce.Model.Models;
using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Categories.GetAll();
            return View(categories);
        }
        public async Task<IActionResult> Create()
        {
            Category category = new();
            return View("CategoryForm", category);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Add(category);
                _unitOfWork.Complete();
                TempData["Success"] = "category created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int? CategoryId)
        {
            var category = _unitOfWork.Categories.Get(x => x.Id == CategoryId);
            if (category == null)
            {
                return NotFound();
            }
            return View("CategoryForm", category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Update(category);
                _unitOfWork.Complete();
                TempData["Success"] = "category Updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int? CategoryId)
        {
            var category = _unitOfWork.Categories.Get(x => x.Id == CategoryId);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Categories.Remove(category);
            _unitOfWork.Complete();
            TempData["Success"] = "category deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
