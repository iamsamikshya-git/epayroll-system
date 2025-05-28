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

  
}