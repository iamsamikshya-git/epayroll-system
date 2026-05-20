using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{
    public class User  // ✅ Add the missing class declaration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }
        
    }
}
