using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Motorak.BLL.ModelVM.Car;
using Motorak.BLL.Services.Abstractions;
using Motorak.BLL.Services.Implementations;
using Motorak.DAl.Repo.Abstractions;

namespace Motorak.PLL.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarServicecs _carService;
        private readonly ICustomerRebo _customerRebo;

        public CarController(ICarServicecs carService, ICustomerRebo customerRebo)
        {
            _carService = carService;
            _customerRebo = customerRebo;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (status, message, cars) = await _carService.GetAllCarsAsync();
            if (!status)
            {
                ViewBag.Error = message;
                return View("Error");
            }

            return View(cars);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var (status, message, car) = await _carService.GetCarByIdAsync(id);
            if (!status || car == null)
                return NotFound(message);

            return View(car);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCarModel car)
        {
            // Add debugging
            if (car == null)
            {
                Console.WriteLine("Model is null!");
                return View();
            }

            Console.WriteLine($"Brand: {car.Brand}, Model: {car.Model}, Year: {car.Year}");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(car);
            }

            var (status, message) = await _carService.CreateCarAsync(car);
            if (!status)
            {
                ModelState.AddModelError("", message);
                return View(car);
            }

            TempData["Success"] = "Car added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var (status, message, car) = await _carService.GetCarByIdAsync(id);
            if (!status || car == null)
            {
                TempData["Error"] = message ?? "Car not found";
                return RedirectToAction(nameof(Index));
            }

            // Map all properties correctly
            var editModel = new EditCarModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                Mileage = car.Mileage,
                Condition = car.Condition,
                Transmission = car.Transmission,
                Price = car.Price,
                Type = car.Type,
                Status = car.Status,      
                Category = car.Category,       
                DailyRentPrice = car.DailyRentPrice,  
                ImagePath = car.ImagePath      
            };

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditCarModel car)
        {
            if (!ModelState.IsValid)
            {
                // Add debug information
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(car);
            }

            var (status, message) = await _carService.UpdateCarAsync(car);
            if (!status)
            {
                ModelState.AddModelError("", message);
                return View(car);
            }

            TempData["Success"] = "Car updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _carService.GetCarByIdAsync(id);

            if (!result.status)
            {
                TempData["ErrorMessage"] = result.message;
                return RedirectToAction("Index");
            }

            return View(result.Car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = await _carService.DeleteCarAsync(id);

                if (result.status)
                {
                    TempData["SuccessMessage"] = result.message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.message;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Sell(int carId)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var customer = await _customerRebo.GetByUsernameAsync(userName);
            if (customer == null)
                return BadRequest("Customer not found.");

            var (status, message) = await _carService.SellCarAsync(carId, customer.Id);
            TempData[status ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rent(int carId)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var customer = await _customerRebo.GetByUsernameAsync(userName);
            if (customer == null)
                return BadRequest("Customer not found.");

            var (status, message) = await _carService.RentCarAsync(carId, customer.Id);
            TempData[status ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}