using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{
    public class School
    {
     public int Id { get; set; }

    [Required]
    public string? SchoolName { get; set; }

    [Required]
    public string? Tole { get; set; }

    [Required]
    public string? TelephoneNo { get; set; }

    [Required]
    public int WardNo { get; set; }

    [Required]
    public string? AccountNo { get; set; }

    [Required]
    public string? ContactPerson { get; set; }

    [Required]
    public string? Email { get; set; }

    [Required]
    public string? UserPerson { get; set; }

    [Required]
    public string? EMISCode { get; set; }

    public int UserId { get; set; }
    public int AdminId { get; set; }

    public User? User { get; set; }
    public Admin? Admin { get; set; }
    }
}
