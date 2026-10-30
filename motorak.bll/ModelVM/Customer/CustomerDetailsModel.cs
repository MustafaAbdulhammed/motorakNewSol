
namespace Motorak.BLL.ModelVM.Customer
{
    public class CustomerDetailsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PurchasedCarsCount { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsUpdated { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
