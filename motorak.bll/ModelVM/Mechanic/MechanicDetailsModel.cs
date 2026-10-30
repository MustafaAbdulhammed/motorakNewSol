
using Motorak.DAL.Enums.MechaincEnums;

namespace Motorak.BLL.ModelVM.Mechanic
{
    public class MechanicDetailsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Rating { get; set; }
        public string WorkHours { get; set; }
        public MechanicStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsUpdated { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
