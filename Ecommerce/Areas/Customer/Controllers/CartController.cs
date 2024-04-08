using Ecommerce.Model.Models;
using Ecommerce.Model.ViewModels;
using Ecommerce.Repositories.IRepositories;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM{ get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var userId = claimidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartViewModel shoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCarts.GetAll(x => x.ApplicationUserId == userId, IncludeProperites: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in shoppingCartViewModel.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(shoppingCartViewModel);  
        }
        
        public IActionResult Summary()
        {
            var ClaimType = (ClaimsIdentity)User.Identity;
            var userId = ClaimType.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel shoppingcartVM= new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCarts.GetAll(x => x.ApplicationUserId == userId, IncludeProperites: "Product"),
                OrderHeader = new()
            };
            shoppingcartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUsers.Get(x => x.Id == userId);
			shoppingcartVM.OrderHeader.ApplicationUserId = userId;
			shoppingcartVM.OrderHeader.Name = shoppingcartVM.OrderHeader.ApplicationUser.Name;
			shoppingcartVM.OrderHeader.PhoneNumber = shoppingcartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingcartVM.OrderHeader.StreetAddress = shoppingcartVM.OrderHeader.ApplicationUser.StreetAddress;
			shoppingcartVM.OrderHeader.City = shoppingcartVM.OrderHeader.ApplicationUser.City;
			shoppingcartVM.OrderHeader.State = shoppingcartVM.OrderHeader.ApplicationUser.State;
			shoppingcartVM.OrderHeader.PostalCode = shoppingcartVM.OrderHeader.ApplicationUser.PostalCode;
			foreach (var cart in shoppingcartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingcartVM.OrderHeader.OrderTotal = shoppingcartVM.OrderHeader.OrderTotal+=(cart.Price*cart.Count);
            }
            return View(shoppingcartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == userId,
                IncludeProperites: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUsers.Get(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
				//it is a regular customer 
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				//it is a company user
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            ShoppingCartVM.OrderHeader.PaymentDate=System.DateTime.Now;
			ShoppingCartVM.OrderHeader.PaymentDueDate = System.DateTime.Now;

			_unitOfWork.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Complete();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = cart.Count,
                    Price = cart.Price,
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Complete();
            }
            // stripe config
            paymentProccess(ShoppingCartVM);

            return new StatusCodeResult(303);
		}


        public IActionResult OrderConfirmation(int id)
        {
            var fromheaderdb = _unitOfWork.OrderHeaders.Get(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(fromheaderdb.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaders.UpdateStatus(fromheaderdb.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCarts.GetAll(x => x.ApplicationUserId == fromheaderdb.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
            _unitOfWork.Complete();
            return View(id);
        }

		public IActionResult Plus(int cartId)
        {
			var cartfromdb = _unitOfWork.ShoppingCarts.Get(x => x.Id == cartId);
            cartfromdb.Count += 1;
            _unitOfWork.ShoppingCarts.Update(cartfromdb);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
        {
            var cartfromdb = _unitOfWork.ShoppingCarts.Get(x => x.Id == cartId);
            if (cartfromdb.Count ==1)
            {
                _unitOfWork.ShoppingCarts.Remove(cartfromdb);
                
            }
            else
            {
                cartfromdb.Count -= 1;
                _unitOfWork.ShoppingCarts.Update(cartfromdb);
            }
			_unitOfWork.Complete();
			return RedirectToAction(nameof(Index));
		}

        public IActionResult Remove(int cartId)
        {
			var cartfromdb = _unitOfWork.ShoppingCarts.Get(x => x.Id == cartId);
            _unitOfWork.ShoppingCarts.Remove(cartfromdb);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCarts
             .GetAll(u => u.ApplicationUserId == cartfromdb.ApplicationUserId).Count() - 1);
            _unitOfWork.Complete();
            
            return RedirectToAction(nameof(Index));


        }




        public void paymentProccess(ShoppingCartViewModel ShoppingCartVM)
        {
            var domain = "https://localhost:7146/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index"
            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)cart.Price * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Title
                        }
                    },
                    Quantity = cart.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaders.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Complete();
            Response.Headers.Add("Location", session.Url);
        }

        public double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
           
        }
    }
}
