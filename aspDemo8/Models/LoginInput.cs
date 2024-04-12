// Demo 8 - Complete Application; LV
// Add this class to create a seperate model for the login credentials provided by the user

using System.ComponentModel.DataAnnotations;

namespace Demo8.Models
{
    public class LoginInput
    {
        [Key]
        [Required(ErrorMessage = "Please enter a username")]
        [MaxLength(20)]
        [Display(Name = "Username")]
        public string? UserLogin { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [UIHint("password")]
        [MaxLength(20)]
        [Display(Name = "Password")]
        public string? UserPassword { get; set; }

        public string? ReturnURL
        {
            get; set;
        }
    }
}
