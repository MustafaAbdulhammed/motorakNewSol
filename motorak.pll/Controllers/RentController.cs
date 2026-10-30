using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motorak.DAL.DataBase;
using Motorak.BLL.ModelVM.Rents;
using Motorak.BLL.Services.Abstractions;
using Motorak.BLL.Services.Implementations;
using System.Security.Claims;

namespace Motorak.PLL.Controllers
{
    [Authorize]  // Require authentication for all actions
    public class RentController : Controller
    {
        private readonly IRentService _service;
        private readonly MotorakDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly ICarServicecs _carService;

        // ✅ Fix: Inject all required services
        public RentController(IRentService service, MotorakDbContext context,
                              ICustomerService customerService, ICarServicecs carService)
        {
            _service = service;
            _context = context;
            _customerService = customerService;
            _carService = carService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Create(int? carId = null, decimal? price = null)
        {
            // Get customers for dropdown (admin only may need this)
            var (status, _, customers) = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");

            var model = new RentCreateDto();
            model.StartDate = DateTime.Today;
            model.EndDate = DateTime.Today.AddDays(1);

            if (carId.HasValue)
            {
                model.CarId = carId.Value;
                var carResult = await _carService.GetCarByIdAsync(carId.Value);
                if (carResult.Car != null)
                {
                    var c = carResult.Car;
                    ViewBag.CarDisplay = $"{c.Brand} {c.Model} ({c.Year})";
                }
                else
                {
                    ViewBag.CarDisplay = "Car not found";
                }
            }
            if (price.HasValue) model.TotalPrice = price.Value;

            // ✅ Fix: Use DbContext directly to find customer by string UserId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                if (customer != null && !User.IsInRole("Admin"))
                {
                    model.CustomerId = customer.Id; // int ← int, works fine
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Create(RentCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null && !User.IsInRole("Admin"))
            {
                ModelState.AddModelError("", "You must be a registered customer to rent a car.");
                return View(dto);
            }

            if (!User.IsInRole("Admin"))
            {
                dto.CustomerId = customer.Id;
            }

            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        [AllowAnonymous] // Anyone can view the list
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            // Get the current logged-in user's Customer record
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // Check if the rent belongs to the current customer OR user is admin
                if (customer != null && item.CustomerId != customer.Id && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                else if (customer == null && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
            }
            else if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var dto = new RentUpdateDto
            {
                Id = existing.Id,
                PaymentMethod = existing.PaymentMethod,
                TotalPrice = existing.TotalPrice,
                Status = existing.Status
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, RentUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();


            ModelState.Remove("CarId");
            ModelState.Remove("CustomerId");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");

            if (!ModelState.IsValid)
            {

                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Database Error: " + ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            return View(existing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // Optional: View current user's rentals
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("Customer profile not found.");
            }

            var allRentals = await _service.GetAllAsync();
            var myRentals = allRentals.Where(r => r.CustomerId == customer.Id).ToList();

            return View(myRentals);
        }
    }
}