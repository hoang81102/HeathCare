using System.ComponentModel.DataAnnotations;

namespace ElderlyCareRazor.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(100)]
        public string Fullname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, RegularExpression(@"^\d{10,12}$", ErrorMessage = "Phone must be 10-12 digits.")]
        public string Phone { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
