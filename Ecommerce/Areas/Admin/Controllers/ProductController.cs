using Ecommerce.Model.Models;
using Ecommerce.Model.ViewModels;
using Ecommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Products = _unitOfWork.Products.GetAll();
            return View(Products);
        }
        public async Task<IActionResult> Create()
        {
            //IEnumerable<SelectListItem> CategoryList = categories.Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});
            ////ViewBag.Categories = CategoryList;
            ////ViewData["Categories"]= CategoryList; // dictionary type
            ///
            IEnumerable<SelectListItem> categories = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            var productvm = new ProductViewModel()
            {
                Product = new Product(),
                Categories = categories
            };
            return View("ProductForm", productvm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel ProductVm)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Products.Add(ProductVm.Product);
                _unitOfWork.Complete();
                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            ProductVm.Categories = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View("ProductForm", ProductVm);
    }

        public IActionResult Edit(int? ProductId)
        {
            var product = _unitOfWork.Products.Get(x => x.Id == ProductId);
            if (product == null)
            {
                return NotFound();
            }
            ProductViewModel productViewModel = new()
            {
                Product = product,
                Categories = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View("ProductForm", productViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ProductViewModel ProductVm)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Products.Update(ProductVm.Product);
                _unitOfWork.Complete();
                TempData["Success"] = "Product Updated successfully";
                return RedirectToAction(nameof(Index));
            }
            ProductVm.Categories = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View("ProductForm", ProductVm);
        }
        public IActionResult Delete(int? ProductId)
        {
            var Product = _unitOfWork.Products.Get(x => x.Id == ProductId);
            if (Product == null)
            {
                return NotFound();
            }
            _unitOfWork.Products.Remove(Product);
            _unitOfWork.Complete();
            TempData["Success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
