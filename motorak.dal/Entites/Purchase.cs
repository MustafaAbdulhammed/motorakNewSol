using motorak.dal.Entites;
using motorak.dal.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motorak.DAL.Entities
{
    public class Purchase : Transactions
    {
        public string SellerName { get;  set; }

        public Purchase(string sellerName, string paymentMethod, decimal totalPrice, int customerId, int carId)
            : base(paymentMethod, totalPrice, customerId, carId)
        {
            SellerName = sellerName;
        }

        private Purchase() { }
    }
}
