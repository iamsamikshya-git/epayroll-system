using E_PayRoll.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace E_PayRoll.ViewModels
{
    public class TeacherListViewModel
    {
        public Teacher? Teacher { get; set; }
        public Admin? Admin { get; set; }
        public School? School { get; set; }

        public IFormFile? PhotoFile { get; set; }
        public IFormFile? CVFile { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        // Role is set in the controller, not by the user/form
        public string Role { get; set; } = "Teacher";
    }
}