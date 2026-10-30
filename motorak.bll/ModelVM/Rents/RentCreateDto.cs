
namespace Motorak.BLL.ModelVM.Rents
{
    public class RentCreateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public int CarId { get; set; }
    }
}
