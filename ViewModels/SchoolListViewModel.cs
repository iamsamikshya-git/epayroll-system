using System.ComponentModel.DataAnnotations;
using E_PayRoll.Models;

namespace E_PayRoll.ViewModels
{
    public class SchoolListViewModel
    {
        [Required(ErrorMessage = "School details are required")]
        public School School { get; set; } = new School();

        [Required(ErrorMessage = "User details are required")]
        public User User { get; set; } = new User();

        public Admin? Admin { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password
        {
            get => User?.Password ?? string.Empty;
            set
            {
                if (User != null)
                    User.Password = value;
            }
        }

        [Required(ErrorMessage = "Username is required")]
        public string Username
        {
            get => User?.Username ?? string.Empty;
            set
            {
                if (User != null)
                    User.Username = value;
            }
        }

        [Required(ErrorMessage = "Role is required")]
        public string Role
        {
            get => User?.Role ?? string.Empty;
            set
            {
                if (User != null)
                    User.Role = value;
            }
        }
    }
}
