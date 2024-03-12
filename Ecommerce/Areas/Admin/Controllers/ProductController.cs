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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var Products = _unitOfWork.Products.GetAll(IncludeProperites:"Category");
            return View(Products);
        }

        public async Task<IActionResult> Upsert(int? ProductId)
        {
            //IEnumerable<SelectListItem> CategoryList = categories.Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});
            ////ViewBag.Categories = CategoryList;
            ////ViewData["Categories"]= CategoryList; // dictionary type
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
            if(ProductId == null || ProductId == 0)
            {
               return View("ProductForm", productvm);
            }
            var product = _unitOfWork.Products.Get(x => x.Id == ProductId);
            productvm.Product = product;
            return View("ProductForm", productvm);
        }


        [HttpPost]
        public async Task<IActionResult> Upsert(ProductViewModel ProductVm,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var filename = AddImage(ProductVm,file);
                    ProductVm.Product.ImageUrl = @"\Images\Products\" + filename;
                }
                if(ProductVm.Product.Id==null || ProductVm.Product.Id == 0)
                {
                    _unitOfWork.Products.Add(ProductVm.Product);
                }
                else
                {
                    _unitOfWork.Products.Update(ProductVm.Product);
                }
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
        
        //public IActionResult Delete(int? ProductId)
        //{
        //    var Product = _unitOfWork.Products.Get(x => x.Id == ProductId);
        //    if (Product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Products.Remove(Product);
        //    _unitOfWork.Complete();
        //    TempData["Success"] = "Product deleted successfully";
        //    return RedirectToAction(nameof(Index));
        //}


        [HttpGet]
        public IActionResult GetAll()
        {
            var Products = _unitOfWork.Products.GetAll(IncludeProperites: "Category");
            return Json(new {data=Products});
        }

        [HttpDelete]
        public IActionResult Remove(int? id)
        {
            var product = _unitOfWork.Products.Get(x=>x.Id== id);
            if(product == null)
            {
                return Json(new { success = false, message = "error while deleting" });
            }
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl);
            if (System.IO.Path.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);    
            }
            _unitOfWork.Products.Remove(product);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "deleted successfully" });
        }



        private string AddImage(ProductViewModel ProductVm,IFormFile file)
        {
            var wwwroot = _webHostEnvironment.WebRootPath;
            string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var Productpath = Path.Combine(wwwroot, @"Images\Products\");

            if (!string.IsNullOrEmpty(ProductVm.Product.ImageUrl))
            {
                var oldpath = Path.Combine(wwwroot, ProductVm.Product.ImageUrl);
                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }
            }
            using (var filestream = new FileStream(Path.Combine(Productpath + filename), FileMode.Create))
            {
                file.CopyTo(filestream);
            }
            return filename;
        }


    }

    
}
