using Ecommerce.Model.Models;
using Ecommerce.Repositories.IRepositories;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Products.GetAll(IncludeProperites: "Category");
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Products.Get(u => u.Id == productId, IncludeProperites: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var userId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            ShoppingCart CartFromDb = _unitOfWork.ShoppingCarts.Get(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);
            if (CartFromDb != null)
            {
                CartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCarts.Update(CartFromDb);
                _unitOfWork.Complete();
            }
            else
            {
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionCart,_unitOfWork.ShoppingCarts.GetAll(x => x.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(Index));
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
