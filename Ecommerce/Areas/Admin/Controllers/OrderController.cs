using Ecommerce.Model.Models;
using Ecommerce.Model.ViewModels;
using Ecommerce.Repositories.IRepositories;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderDetailsViewModel orderDetailsVM { get; set; }  // changes listened in all the actions in controller
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            orderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == orderId, IncludeProperites: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderHeaderId == orderId, IncludeProperites: "Product")
            };
            return View(orderDetailsVM);
        }

        [HttpGet]
        public IActionResult GetOrderList(string status) 
        {
            var claimientity = (ClaimsIdentity)User.Identity;
            var userId = claimientity.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(IncludeProperites: "ApplicationUser");
            }
            else
            {
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(x => x.ApplicationUserId == userId, IncludeProperites: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusApproved || x.OrderStatus == SD.StatusInProcess || x.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusCancelled || x.OrderStatus == SD.StatusRefunded || x.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;

            }

            return Json(new { data = orderHeaders });
        }

        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == orderDetailsVM.OrderHeader.Id);
            orderHeader.TrackingNumber = orderDetailsVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderDetailsVM.OrderHeader.Carrier;
            orderHeader.OrderStatus= SD.StatusShipped;
            orderHeader.ShippingDate= DateTime.Now;
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index)); 
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaders.Get(x => x.Id == id);
            if(orderHeader.PaymentStatus == SD.StatusApproved)
            {
                var option = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.SessionId,
                };
                var sercice = new RefundService();
                Refund refund = sercice.Create(option);
                orderHeader.OrderStatus = SD.StatusRefunded;
                orderHeader.PaymentStatus = SD.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
