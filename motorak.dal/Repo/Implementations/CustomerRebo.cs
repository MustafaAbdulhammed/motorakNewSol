
using Microsoft.EntityFrameworkCore;
using motorak.dal.Entites;
using motorak.dal.Migrations;
using motorak.DAL.DataBase;
using Motorak.DAl.Repo.Abstractions;
using Motorak.DAL.Entites;

namespace Motorak.DAl.Repo.Implementations
{
    public class CustomerRebo : ICustomerRebo
    {
        private readonly MotorakDbContext MDB;

        public CustomerRebo(MotorakDbContext MDB)
        {
            this.MDB = MDB;
        }

        public async Task CreateAsync(Customer customer)
        {
            await MDB.Customers.AddAsync(customer);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await MDB.Customers.Include(u => u.User).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await MDB.Customers
                .Include(c => c.User)   // load related User so it can be updated
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<Customer?> GetByUserIdAsync(string id)
        {
            return await MDB.Customers.Include(u=>u.User).Include(u => u.Purchases).Where(u=>u.UserId == id).FirstOrDefaultAsync();
        }
        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await MDB.Customers.Include(u => u.User).Where(u => u.User.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await MDB.Customers.Include(u => u.User).Where(u => u.User.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            return await MDB.Customers.Include(u => u.User).Where(u => u.User.PhoneNumber == phone).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await MDB.SaveChangesAsync();
        }

        public void Update(Customer customer)
        {
            MDB.Customers.Update(customer);
        }

        public void Delete(Customer customer)
        {
            customer.Delete();
            MDB.Customers.Update(customer);
        }

    }
}
