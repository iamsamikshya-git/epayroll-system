using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models;

public class School
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "";
    public string? Address { get; set; }
}