using System.ComponentModel.DataAnnotations;

namespace ElderlyCareRazor.Models
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Fullname { get; set; } = string.Empty;

        public string? Address { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Hobby { get; set; }
        public string AccountStatus { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

}
