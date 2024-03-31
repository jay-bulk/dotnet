// Demo 7 - Shhopping Cart; LV
// Add this class to create a seperate model for the login credentials provided by the user

using System.ComponentModel.DataAnnotations;

namespace Demo7.Models
{
    public class LoginInput
    {
        [Key]
        [Required(ErrorMessage = "Please enter a username")]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [UIHint("password")]
        [MaxLength(50)]
        [Display(Name = "Password")]
        public string? UserPassword { get; set; }

        public string? ReturnURL
        {
            get; set;
        }
    }
}
