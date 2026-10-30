using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motorak.BLL.ModelVM.Customer;
using Motorak.BLL.ModelVM.Mechanic;
using Motorak.BLL.Services.Abstractions;
using Motorak.BLL.Services.Implementations;
using Motorak.Utility;

namespace Motorak.PLL.Controllers
{
    [Authorize(Roles = Seed.Role_Admin)]
    public class MechanicController : Controller
    {
        private readonly IMechanicService _mechanicService;
        public MechanicController(IMechanicService mechanicService)
        {
            _mechanicService = mechanicService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (status, message, mechanics) = await _mechanicService.GetAllMechanicsAsync();
            if (!status || mechanics == null)
            {
                ViewBag.Error = message;
                return View("No Mechanics Found");
            }
            return View(mechanics);
        }


        [HttpGet]
        public async Task<IActionResult> MechanicDetails(int id)
        {
            var (status, message, mechanic) = await _mechanicService.GetMechanicByIdAsync(id);
            if (!status || mechanic == null)
            {
                ViewBag.Error = message;
                return View("Mechanic Not Found");
            }
            return View(mechanic);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMechanicModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (status, message) = await _mechanicService.CreateMechanicAsync(model);
            if (!status)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var (status, message, existing) = await _mechanicService.GetMechanicByIdAsync(id);
            if (!status || existing == null)
            {
                ViewBag.Error = message;
                return NotFound(message);
            }

            if (existing.IsDeleted)
            {
                ViewBag.Error = "This mechanic has been deleted.";
                return View("Mechanic Deleted");
            }

            var model = new EditMechanicModel
            {
                Id = existing.Id,
                Name = existing.Name,
                WorkHours = existing.WorkHours,
                Status = existing.Status

            };
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMechanicModel model)
        {
            try
            {
                await _mechanicService.EditMechanicAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (status, message) = await _mechanicService.DeleteMechanicAsync(id);
            if (!status)
            {
                TempData["Error"] = message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Mechanic Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}

