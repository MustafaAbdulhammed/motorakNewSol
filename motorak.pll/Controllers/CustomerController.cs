using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motorak.BLL.ModelVM.Customer;
using Motorak.BLL.Services.Abstractions;
using Motorak.Utility;

namespace Motorak.PLL.Controllers
{
    [Authorize(Roles = Seed.Role_Admin)]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var (status, message, customers) = await _customerService.GetAllCustomersAsync();
                if (!status || customers == null)
                {
                    ViewBag.Error = message ?? "No customers found";
                    return View(new List<CustomerListModel>());
                }
                return View(customers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while loading customers: " + ex.Message;
                return View(new List<CustomerListModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            try
            {
                var (status, message, customer) = await _customerService.GetCustomerByIdAsync(id);
                if (!status || customer == null)
                {
                    TempData["Error"] = message ?? "Customer not found";
                    return RedirectToAction(nameof(Index));
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading customer details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCustomerModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var (status, message) = await _customerService.CreateCustomerAsync(model);
                if (!status)
                {
                    ModelState.AddModelError("", message ?? "Failed to create customer");
                    return View(model);
                }

                TempData["Success"] = "Customer created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the customer: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var (status, message, existing) = await _customerService.GetCustomerByIdAsync(id);
                if (!status || existing == null)
                {
                    TempData["Error"] = message ?? "Customer not found";
                    return RedirectToAction(nameof(Index));
                }

                if (existing.IsDeleted)
                {
                    TempData["Error"] = "Cannot edit a deleted customer";
                    return RedirectToAction(nameof(Index));
                }

                var model = new EditCustomerModel
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    PhoneNumber = existing.PhoneNumber,
                    UpdatedAt = DateTime.Now,
                    IsUpdated = true
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading customer for editing: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCustomerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                model.UpdatedAt = DateTime.Now;
                model.IsUpdated = true;

                var (status, message) = await _customerService.EditCustomerAsync(model);
                if (!status)
                {
                    ModelState.AddModelError("", message ?? "Failed to update customer");
                    return View(model);
                }

                TempData["Success"] = "Customer updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the customer: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var (status, message) = await _customerService.DeleteCustomerAsync(id);
                if (!status)
                {
                    TempData["Error"] = message ?? "Failed to delete customer";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Customer deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the customer: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Restore(int id)
        //{
        //    try
        //    {
        //        
        //       
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "An error occurred while restoring the customer: " + ex.Message;
        //        return RedirectToAction(nameof(Index));
        //    }
        //}
    }
}