using Ecommerce.Repositories.Data;
using Ecommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork=unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var userlist = _context.ApplicationUsers.Include(x => x.Company).ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            foreach (var user in userlist)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                if (roleId == null)
                {
                    continue;
                }
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Model.Models.Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = userlist });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.ApplicationUsers.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUsers.Update(objFromDb);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Operation Successful" });
        }

    }
}
