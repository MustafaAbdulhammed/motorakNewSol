
using System.ComponentModel.DataAnnotations;
using Motorak.DAL.Enums.MechaincEnums;

namespace Motorak.BLL.ModelVM.Mechanic
{
    public class EditMechanicModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ImagePath { get; set; }

        [Required, Display(Name = "Working Hours")]
        public string WorkHours { get; set; }

        [Required]
        public decimal Rating { get; set; }

        [Required]
        public MechanicStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsUpdated { get; set; } = true;
    }
}
