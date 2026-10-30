

//using Motorak.DAL.Enums.ServiceReviewEnums;
using motorak.dal.Entites;
using Motorak.DAL.Enums.SeviceEnums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Motorak.DAL.Entites
{
    public class Service
    {
        public int Id { get; private set; }
        public DateTime RequestDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public ServiceType ServiceType { get; private set; }
        public Status Status { get; private set; }
        public int? CustomerId { get; private set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; private set; }
        public int MechanicId { get; private set; }
        [ForeignKey(nameof(MechanicId))]
        public virtual Mechanic Mechanic { get; private set; }
        public int CarId { get; private set; }
        [ForeignKey(nameof(CarId))]
        public virtual Car Car { get; private set; }


        public virtual List<ServiceReview>? ServiceReviews { get; set; }

        // In motorak.dal/Entites/Service.cs constructor, update the default:
        public Service(
            DateTime requestDate,
            ServiceType serviceType,
            int customerId,
            int mechanicId,
            int carId)
        {
            RequestDate = requestDate;
            CreatedAt = DateTime.Now;
            IsDeleted = false;
            ServiceType = serviceType;
            Status = Status.Pending;  // ✅ was Status.Pending (already correct, but now enum only has one Pending)
            CustomerId = customerId;
            MechanicId = mechanicId;
            CarId = carId;
        }

        public Service()
        {
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }
        public void UpdateStatus(Status status)
        {
            Status = status;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateServiceType(ServiceType serviceType)
        {
            ServiceType = serviceType;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateRequestDate(DateTime requestDate)
        {
            RequestDate = requestDate;
            UpdatedAt = DateTime.Now;
        }
    }
}
