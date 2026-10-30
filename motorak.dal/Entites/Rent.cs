using motorak.dal.Entites;
using motorak.dal.Entities;
using Motorak.DAL.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motorak.DAL.Entities
{
    public class Rent : Transactions
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public Rent(DateTime startDate, DateTime endDate, string paymentMethod, decimal totalPrice, int customerId, int carId)
            : base(paymentMethod, totalPrice, customerId, carId)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        private Rent() { }
    }
}
