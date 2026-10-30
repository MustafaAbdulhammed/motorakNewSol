// IRentRepo.cs
using Motorak.DAL.Entities;


public interface IRentRepo
{
    Task<Rent?> GetByIdAsync(int id);
    Task<List<Rent>> GetAllAsync();
    Task AddAsync(Rent rent);
    Task UpdateAsync(Rent rent);
    Task DeleteAsync(int id);

    // New method: check if car is available for given dates
    Task<bool> IsCarAvailableForDates(int carId, DateTime startDate, DateTime endDate);
}