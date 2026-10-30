// motorak.dal/Entites/Transactions.cs
using motorak.dal.Entites;
using Motorak.DAL.Enums.TransactionEnums;
using System.ComponentModel.DataAnnotations.Schema;

namespace motorak.dal.Entities
{
    public class Transactions
    {
        public int Id { get; private set; }
        public string PaymentMethod { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal TotalPrice { get; private set; }
        public TransactionStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public string CreatedBy { get;  set; } = "System";

        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get;  set; }  // ← changed to protected

        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string? DeletedBy { get; private set; }

        public int CustomerId { get; private set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; private set; }

        public int CarId { get; private set; }
        [ForeignKey(nameof(CarId))]
        public virtual Car Car { get; private set; }

        public Transactions() { }

        public Transactions(string paymentMethod, decimal totalPrice, int customerId, int carId, string createdBy = "System")
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("Payment method is required.");
            if (totalPrice < 0)
                throw new ArgumentException("Price cannot be negative.");

            PaymentMethod = paymentMethod;
            TotalPrice = totalPrice;
            TransactionDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            CreatedBy = createdBy;
            Status = TransactionStatus.Pending;
            CustomerId = customerId;
            CarId = carId;
            IsDeleted = false;
        }

        public void MarkAsCompleted(string updatedBy = "System")
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Only pending transactions can be completed.");
            Status = TransactionStatus.Completed;
            UpdatedAt = DateTime.Now;
            UpdatedBy = updatedBy;
        }

        public void Cancel(string updatedBy = "System")
        {
            if (Status == TransactionStatus.Completed)
                throw new InvalidOperationException("Completed transactions cannot be cancelled.");
            Status = TransactionStatus.Cancelled;
            UpdatedAt = DateTime.Now;
            UpdatedBy = updatedBy;
        }

        public void UpdatePrice(decimal newPrice, string updatedBy = "System")
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative.");
            TotalPrice = newPrice;
            UpdatedAt = DateTime.Now;
            UpdatedBy = updatedBy;
        }

        public void ChangePaymentMethod(string newMethod, string updatedBy = "System")
        {
            if (string.IsNullOrWhiteSpace(newMethod))
                throw new ArgumentException("Payment method cannot be empty.");
            PaymentMethod = newMethod;
            UpdatedAt = DateTime.Now;
            UpdatedBy = updatedBy;
        }

        public void Delete(string deletedBy = "System")
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
            DeletedBy = deletedBy;
        }

        public void UpdateTransaction(string? newPaymentMethod, decimal? newPrice, TransactionStatus? newStatus, string updatedBy = "System")
        {
            if (newPrice.HasValue)
                UpdatePrice(newPrice.Value, updatedBy);

            if (!string.IsNullOrWhiteSpace(newPaymentMethod))
                ChangePaymentMethod(newPaymentMethod, updatedBy);

            // Only change status if it's different and allowed
            if (newStatus.HasValue && newStatus.Value != Status)
            {
                if (newStatus.Value == TransactionStatus.Completed)
                    MarkAsCompleted(updatedBy);
                else if (newStatus.Value == TransactionStatus.Cancelled)
                    Cancel(updatedBy);
            }

            UpdatedAt = DateTime.Now;
            UpdatedBy = updatedBy;
        }
    }
}