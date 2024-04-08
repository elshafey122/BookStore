using Ecommerce.Model.Models;
using Ecommerce.Repositories.IRepositories;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var companies = _unitOfWork.Companies.GetAll();
            return View(companies);
        }
        public async Task<IActionResult> Create()
        {
            Company company = new();
            return View("CompanyForm", company);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Company company)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Companies.Add(company);
                _unitOfWork.Complete();
                TempData["Success"] = "company created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int? CompanyId)
        {
            var compony = _unitOfWork.Companies.Get(x => x.Id == CompanyId);
            if (compony == null)
            {
                return NotFound();
            }
            return View("CompanyForm", compony);
        }
        [HttpPost]
        public IActionResult Edit(Company company)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Companies.Update(company);
                _unitOfWork.Complete();
                TempData["Success"] = "compony Updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int? CompanyId)
        {
            var compony = _unitOfWork.Companies.Get(x => x.Id == CompanyId);
            if (compony == null)
            {
                return NotFound();
            }
            _unitOfWork.Companies.Remove(compony);
            _unitOfWork.Complete();
            TempData["Success"] = "compony deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
