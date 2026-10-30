// motorak.dal/Repo/Implementations/RentRepo.cs
using Microsoft.EntityFrameworkCore;
using motorak.DAL.DataBase;
using Motorak.DAL.Entities;
using Motorak.DAL.Repo.Abstractions;

namespace Motorak.DAL.Repo.Implementations
{
    public class RentRepo : IRentRepo
    {
        private readonly MotorakDbContext _context;

        public RentRepo(MotorakDbContext context)
        {
            _context = context;
        }

        public async Task<Rent?> GetByIdAsync(int id)
        {
            return await _context.Rents.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<Rent>> GetAllAsync()
        {
            return await _context.Rents.Where(r => !r.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(Rent rent)
        {
            await _context.Rents.AddAsync(rent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rent rent)
        {
            // rent is already tracked by the context (fetched in service)
            // Just mark it as modified and save.
            _context.Rents.Update(rent);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rent = await _context.Rents
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
            if (rent == null)
                throw new InvalidOperationException("Rent not found.");

            rent.Delete();
            await _context.SaveChangesAsync();
        }
        // RentRepo.cs
        public async Task<bool> IsCarAvailableForDates(int carId, DateTime startDate, DateTime endDate)
        {
            // Check for any overlapping, non-deleted rentals
            var overlapping = await _context.Rents
                .Where(r => r.CarId == carId && !r.IsDeleted &&
                            ((startDate >= r.StartDate && startDate < r.EndDate) ||
                             (endDate > r.StartDate && endDate <= r.EndDate) ||
                             (startDate <= r.StartDate && endDate >= r.EndDate)))
                .AnyAsync();

            return !overlapping; // return true if available
        }
    }
}