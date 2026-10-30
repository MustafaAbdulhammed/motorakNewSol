

namespace Motorak.BLL.ModelVM.Customer
{
    public class CustomerListModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int PurchasedCarsCount { get; set; }
        public DateTime CreatedAt { get; set; } 
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsUpdated { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}
