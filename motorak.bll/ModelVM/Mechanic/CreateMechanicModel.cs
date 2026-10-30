
using System.ComponentModel.DataAnnotations;

namespace Motorak.BLL.ModelVM.Mechanic
{
    public class CreateMechanicModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress, MinLength(8)]
        public string Email { get; set; }
        
        public string? PhoneNumber { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password),Display(Name="Confirm Password"),Compare("Password",ErrorMessage="Not matched with Password")]
        public string ConfirmPassword { get; set; }

        [Required,Display(Name ="Working Hours")]
        public string WorkHours { get; set; }

        [Required]
        public string Status { get; set; } = "Free";
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
