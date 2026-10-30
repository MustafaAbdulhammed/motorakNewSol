// motorak.dal/Repo/Implementations/PurchaseRepo.cs
using Microsoft.EntityFrameworkCore;
using motorak.DAL.DataBase;
using Motorak.DAL.Entities;
using Motorak.DAL.Repo.Abstractions;

namespace Motorak.DAL.Repo.Implementations
{
    public class PurchaseRepo : IPurchaseRepo
    {
        private readonly MotorakDbContext _context;

        public PurchaseRepo(MotorakDbContext context)
        {
            _context = context;
        }

        public async Task<Purchase?> GetByIdAsync(int id)
        {
            return await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<List<Purchase>> GetAllAsync()
        {
            return await _context.Purchases.Where(p => !p.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(Purchase purchase)
        {
            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Purchase purchase)
        {
            var existing = await _context.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchase.Id && !p.IsDeleted);
            if (existing == null)
                throw new InvalidOperationException("Purchase not found.");

            existing.UpdateTransaction(
                purchase.PaymentMethod,
                purchase.TotalPrice,
                purchase.Status,
                "System"  
            );
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (purchase == null)
                throw new InvalidOperationException("Purchase not found.");

            purchase.Delete();
            await _context.SaveChangesAsync();
        }
    }
}