using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
    [Required]
    public string Role { get; set; } = "";

    // Add these for admin creation
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? LocalBodyName { get; set; }
    public string? LocalBodyType { get; set; }
    public string? District { get; set; }
    public string? Email { get; set; }
    public string? LogoPath { get; set; } // To store the file path of the uploaded logo
}