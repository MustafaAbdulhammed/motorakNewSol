
using Microsoft.EntityFrameworkCore;
using motorak.dal.Entites;
using motorak.DAL.DataBase;
using Motorak.DAL.Entites;
using Motorak.DAL.Repo.Abstractions;

namespace Motorak.DAL.Repo.Implementations
{
    public class MechanicRebo : IMechanicRebo
    {
        private readonly MotorakDbContext MDB;

        public MechanicRebo(MotorakDbContext MDB)
        {
            this.MDB = MDB;
        }

        public async Task AddAsync(Mechanic mechanic)
        {
            await MDB.Mechanics.AddAsync(mechanic);
        }

        public async Task<List<Mechanic>> GetAllAsync()
        {
            return await MDB.Mechanics.Include(m => m.User).ToListAsync();
        }

        public async Task<Mechanic?> GetByIdAsync(int id)
        {
            return await MDB.Mechanics.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await MDB.SaveChangesAsync();
        }

        public void Update(Mechanic mechanic)
        {
            MDB.Mechanics.Update(mechanic);
        }

        public void Delete(Mechanic mechanic)
        {
            mechanic.Delete();
            MDB.Mechanics.Update(mechanic);
        }
    }
}
