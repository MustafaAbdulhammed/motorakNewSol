// motorak.bll/Services/Implementations/CarService.cs
using AutoMapper;
using motorak.dal.Entites;
using motorak.dal.Entities;
using motorak.dal.Enums.CarEnums;
using Motorak.BLL.Helper;
using Motorak.BLL.ModelVM.Car;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAl.Repo.Abstractions;
using Motorak.DAL.Entites;
using Motorak.DAL.Entities;
using Motorak.DAL.Enums.CarEnums;
using Motorak.DAL.Repo.Abstractions;

namespace Motorak.BLL.Services.Implementations
{
    public class CarService : ICarServicecs
    {
        private readonly ICarRebo _carRebo;
        private readonly ICustomerRebo _customerRebo;
        private readonly IPurchaseRepo _purchaseRepo;
        private readonly IRentRepo _rentRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMapper _mapper;

        public CarService(ICarRebo carRebo, ICustomerRebo customerRebo, IMapper mapper,
            IPurchaseRepo purchaseRepo, IRentRepo rentRepo, ITransactionRepo transactionRepo)
        {
            _carRebo = carRebo;
            _customerRebo = customerRebo;
            _mapper = mapper;
            _purchaseRepo = purchaseRepo;
            _rentRepo = rentRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<(bool status, string messagee)> CreateCarAsync(CreateCarModel car)
        {
            try
            {
                if (car == null) return (false, "Car data is null");

                

                var result = _mapper.Map<Car>(car);
                await _carRebo.CreateAsync(result);
                await _carRebo.SaveChangesAsync();
                return (true, "Car Created Successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error Occurred: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> DeleteCarAsync(int id)
        {
            try
            {
                var result = await _carRebo.GetByIdAsync(id);
                if (result == null)
                    return (false, "Car not Found");

                _carRebo.Delete(result);
                var saveResult = await _carRebo.SaveChangesAsync();

                return saveResult > 0
                    ? (true, "Car deleted successfully")
                    : (false, "Failed to delete car - no changes were made");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete due to: {ex.Message}");
            }
        }

        public async Task<(bool status, string message, List<CarListModel> Cars)> GetAllCarsAsync()
        {
            try
            {
                var cars = await _carRebo.GetAllAsync();
                if (cars == null)
                    return (false, "Could not retrieve car data.", new List<CarListModel>());

                var result = _mapper.Map<List<CarListModel>>(cars);
                return (true, "Cars retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to retrieve cars: {ex.Message}", new List<CarListModel>());
            }
        }

        public async Task<(bool status, string message, CarDetailsModel? Car)> GetCarByIdAsync(int id)
        {
            try
            {
                var car = await _carRebo.GetByIdAsync(id);
                if (car == null) return (false, "Car not Found", null);
                var result = _mapper.Map<CarDetailsModel>(car);
                return (true, "Car retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to retrieve car: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message)> RentCarAsync(int carId, int customerId)
        {
            try
            {
                var car = await _carRebo.GetByIdAsync(carId);
                if (car == null)
                    return (false, "Car not found");

                if (car.Status != CarStatus.Available)
                    return (false, "Car is not available");

                // ✅ Fixed: check ForRent (and Both) correctly
                if (car.Category != CarCategory.ForRent && car.Category != CarCategory.Both)
                    return (false, "Car is not available for rent");

                car.RentToCustomer(customerId);
                _carRebo.Update(car);

                var rent = new Rent(
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(10),
                    paymentMethod: "Online",
                    totalPrice: car.DailyRentPrice ?? car.Price,  // ✅ use DailyRentPrice if available
                    customerId: customerId,
                    carId: carId
                );
                await _rentRepo.AddAsync(rent);

                var trans = new Transactions(
                    paymentMethod: "Online",
                    totalPrice: car.DailyRentPrice ?? car.Price,
                    customerId: customerId,
                    carId: carId
                );
                await _transactionRepo.AddAsync(trans);

                await _carRebo.SaveChangesAsync();
                return (true, "Car rented successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to rent car: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> SellCarAsync(int carId, int customerId)
        {
            try
            {
                var car = await _carRebo.GetByIdAsync(carId);
                if (car == null)
                    return (false, "Car not found");

                if (car.Status != CarStatus.Available)
                    return (false, "Car is not available");

                // ✅ Fixed: check ForSale (and Both) correctly
                if (car.Category != CarCategory.ForSale && car.Category != CarCategory.Both)
                    return (false, "Car is not available for purchase");

                car.SellToCustomer(customerId);
                _carRebo.Update(car);

                var purchase = new Purchase(
                    sellerName: "Motorak Admin",
                    paymentMethod: "Cash",
                    totalPrice: car.Price,  // ✅ decimal, no cast needed
                    customerId: customerId,
                    carId: carId
                );
                await _purchaseRepo.AddAsync(purchase);

                var trans = new Transactions(
                    paymentMethod: "Cash",
                    totalPrice: car.Price,  // ✅ keep as decimal
                    customerId: customerId,
                    carId: carId
                );
                await _transactionRepo.AddAsync(trans);

                await _carRebo.SaveChangesAsync();
                return (true, "Car sold successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to sell car: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> UpdateCarAsync(EditCarModel carModel)
        {
            try
            {
                if (carModel == null)
                    return (false, "Car data is null");

                var existingCar = await _carRebo.GetByIdAsync(carModel.Id);
                if (existingCar == null)
                    return (false, "Car not found");

                existingCar.Edit(
                    brand: carModel.Brand,
                    model: carModel.Model,
                    year: carModel.Year,
                    color: carModel.Color,
                    mileage: carModel.Mileage,
                    type: carModel.Type,
                    transmission: carModel.Transmission,
                    condition: carModel.Condition,
                    status: carModel.Status,
                    category: carModel.Category,
                    price: carModel.Price,
                    dailyRentPrice: carModel.DailyRentPrice,
                    imagePath: carModel.ImagePath
                );
                existingCar.Update();
                _carRebo.Update(existingCar);

                var saveResult = await _carRebo.SaveChangesAsync();
                return saveResult > 0
                    ? (true, "Car updated successfully")
                    : (false, "No changes were saved to the database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateCarAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return (false, $"Failed to update car: {ex.Message}");
            }
        }
    }
}