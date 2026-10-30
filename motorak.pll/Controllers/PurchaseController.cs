using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motorak.DAL.DataBase;
using Motorak.BLL.ModelVM.Purchases;
using Motorak.BLL.Services.Abstractions;
using Motorak.BLL.Services.Implementations;
using System.Security.Claims;


namespace Motorak.PLL.Controllers
{
    [Authorize]  // Require authentication for all actions
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _service;
        private readonly ICustomerService _customerService;
        private readonly ICarServicecs _carService;
        private readonly MotorakDbContext _context;

        public PurchaseController(IPurchaseService service, ICustomerService customerService, ICarServicecs carService, MotorakDbContext context)
        {
            _service = service;
            _customerService = customerService;
            _carService = carService;
            _context = context;
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
            return View(item);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Create(int? carId = null, decimal? price = null)
        {
            var (status, _, customers) = await _customerService.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");

            var sellers = new List<string> { "Motorak Store", "Private Seller", "Dealership", "Auction" };
            ViewBag.Sellers = new SelectList(sellers);

            var model = new PurchaseCreateDto();
            if (carId.HasValue)
            {
                model.CarId = carId.Value;
                var carResult = await _carService.GetCarByIdAsync(carId.Value);
                if (carResult.Car != null)
                {
                    var c = carResult.Car;
                    ViewBag.CarDisplay = $"{c.Brand} {c.Model} ({c.Year})";
                }
                else ViewBag.CarDisplay = "Car not found";
            }
            if (price.HasValue) model.TotalPrice = price.Value;

            // ✅ NEW: Auto-set CustomerId for non-admin users
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                if (customer != null && !User.IsInRole("Admin"))
                {
                    model.CustomerId = customer.Id;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Create(PurchaseCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (!User.IsInRole("Admin"))
            {
                if (customer == null)
                {
                    ModelState.AddModelError("", "You must be a registered customer to make a purchase.");
                    return View(dto);
                }
                dto.CustomerId = customer.Id;
            }

            if (!ModelState.IsValid) return View(dto);

            await _service.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var dto = new PurchaseUpdateDto
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
        public async Task<IActionResult> Edit(int id, PurchaseUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
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
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}