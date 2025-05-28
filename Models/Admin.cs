using System;

namespace E_PayRoll.Models;

public class Admin
{

    public int Id { get; set; }
    public int UserId { get; set; } // Foreign key to User
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? LocalBodyName { get; set; }
    public string? LocalBodyType { get; set; }
    public string? District { get; set; }
    public string? Email { get; set; }
    public string? LogoPath { get; set; }

    // Navigation property
    public User? User { get; set; }
}


