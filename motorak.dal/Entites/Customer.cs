using Motorak.DAL.Entities;
using Motorak.DAL.Entites;

namespace motorak.dal.Entites
{
    public class Customer
    {
        public int Id { get; set; }
        public virtual List<Purchase>? Purchases { get; set; } = new List<Purchase>();
        public virtual List<Rent>? Rents { get; set; } = new List<Rent>();
        public virtual List<Service>? Services { get; set; } = new List<Service>();
        public virtual List<ServiceReview>? ServiceReviews { get; set; } = new List<ServiceReview>();
        public virtual List<Car>? Cars { get; set; } = new List<Car>();

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedAt { get;  set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsUpdated { get; private set; }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }
        public void UpdateCustomerInfo(string name, string phonenumber)
        {
            if (User != null)
            {
                User.Name = name;
                User.PhoneNumber = phonenumber;
            }
            UpdatedAt = DateTime.Now;
            IsUpdated = true;
        }

        public Customer()
        {
            Purchases = new List<Purchase>();
            CreatedAt = DateTime.Now;
            IsDeleted = false;
            IsUpdated = false;

        }

    }
}