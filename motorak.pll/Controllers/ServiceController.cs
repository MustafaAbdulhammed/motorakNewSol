using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motorak.BLL.ModelVM.Service;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAL.Enums.SeviceEnums;
using Motorak.Utility;

namespace Motorak.PLL.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceServicecs _serviceService;

        public ServiceController(IServiceServicecs serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (status, message, services) = await _serviceService.GetAllServicesAsync();

            if (!status)
            {
                ViewBag.ErrorMessage = message;
                return View(new List<ServiceDTO>());
            }

            return View(services);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var (status, message, service) = await _serviceService.GetServiceByIdAsync(id);
            if (!status || service == null)
            {
                ViewBag.ErrorMessage = message;
                return NotFound();
            }
            return View(service);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (status, message) = await _serviceService.CreateServiceAsync(model);
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
            var (status, message, service) = await _serviceService.GetServiceByIdAsync(id);

            if (!status || service == null)
                return NotFound(message);


            var model = new UpdateServiceVM
            {
                ServiceId = service.ServiceId, 
                RequestDate = service.CreatedDate,
                Status = service.Status,
                ServiceType = service.ServiceType
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateServiceVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (status, message) = await _serviceService.UpdateServiceStatusAsync(model);
            if (!status)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var (status, message, service) = await _serviceService.GetServiceByIdAsync(id);
            if (!status || service == null) return NotFound(message);
            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int serviceId)
        {
            var (status, message) = await _serviceService.DeleteServiceAsync(serviceId);
            if (status)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = message;
            return RedirectToAction(nameof(Delete), new { id = serviceId });
        }
        [HttpGet]
        [Authorize(Roles = Seed.Role_Mechanic)]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                var (getStatus, getMessage, service) = await _serviceService.GetServiceByIdAsync(id);

                if (!getStatus || service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                if (service.Status != Status.Pending)
                {
                    TempData["ErrorMessage"] = "This service is not pending and cannot be accepted.";
                    return RedirectToAction("Index");
                }

                var updateModel = new UpdateServiceVM
                {
                    ServiceId = service.ServiceId,
                    Status = Status.Completed,
                    RequestDate = service.CreatedDate,
                    ServiceType = service.ServiceType
                };

                var (updateStatus, updateMessage) = await _serviceService.UpdateServiceStatusAsync(updateModel);

                if (!updateStatus)
                {
                    TempData["ErrorMessage"] = updateMessage;
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = $"Service #{service.ServiceId} has been accepted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while accepting the service. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = Seed.Role_Mechanic)]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var (getStatus, getMessage, service) = await _serviceService.GetServiceByIdAsync(id);

                if (!getStatus || service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Index");
                }

                if (service.Status != Status.Pending)
                {
                    TempData["ErrorMessage"] = "This service is not pending and cannot be rejected.";
                    return RedirectToAction("Index");
                }

                var updateModel = new UpdateServiceVM
                {
                    ServiceId = service.ServiceId,
                    Status = Status.Cancelled,
                    RequestDate = service.CreatedDate,
                    ServiceType = service.ServiceType
                };

                var (updateStatus, updateMessage) = await _serviceService.UpdateServiceStatusAsync(updateModel);

                if (!updateStatus)
                {
                    TempData["ErrorMessage"] = updateMessage;
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = $"Service #{service.ServiceId} has been rejected.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while rejecting the service. Please try again.";
                return RedirectToAction("Index");
            }
        }

    }
}
