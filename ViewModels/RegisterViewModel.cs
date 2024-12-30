using System.ComponentModel.DataAnnotations;

namespace DierenManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string GeneratedPassword { get; set; } = string.Empty; // de view verwacht value
    }
}
