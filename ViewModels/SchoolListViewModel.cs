using System.ComponentModel.DataAnnotations;
using E_PayRoll.Models;

namespace E_PayRoll.ViewModels
{
    public class SchoolListViewModel
    {
        public School School { get; set; } = new School();
        public User User { get; set; } = new User();
        public Admin? Admin { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        public string? Username
        {
            get => User?.Username;
            set { if (User != null) User.Username = value; }
        }

        public string? Role
        {
            get => User?.Role;
            set { if (User != null) User.Role = value; }
        }
public SchoolListViewModel()
    {
        Role = "School";
    }
    }
}