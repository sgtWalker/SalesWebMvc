using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private const string DEFAULT_MESSAGE_NOT_FOUND = "Seller not found";
        private const string DEFAULT_MESSAGE_NOT_PROVIDED = "The SellerId not provided";
        private const string DEFAULT_MESSAGE_NOT_EQUALS = "The SellerId not equals the Seller in object";
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await SearchSellerAsync(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sellerService.RemoveAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntegrityException ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"{nameof(IntegrityException)}: {ex.Message}" });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            return await SearchSellerAsync(id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            return await SearchSellerAsync(id, true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = DEFAULT_MESSAGE_NOT_EQUALS });
            }

            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"{nameof(NotFoundException)}: {ex.Message}"});
            }
            catch (DbConcurrencyException ex)
            {
                return RedirectToAction(nameof(Error), new { message = $"{nameof(DbConcurrencyException)}: {ex.Message}" });
            }
        }

        private async Task<IActionResult> SearchSellerAsync(int? id, bool IsEdit = false)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = DEFAULT_MESSAGE_NOT_PROVIDED });
            }

            var seller = await _sellerService.FindByIdAsync(id.Value);
            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = DEFAULT_MESSAGE_NOT_FOUND });
            }

            if (IsEdit)
            {
                List<Department> departments = await _departmentService.FindAllAsync();
                SellerFormViewModel viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };

                return View(viewModel);
            }
            else
            {
                return View(seller);
            }
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}
