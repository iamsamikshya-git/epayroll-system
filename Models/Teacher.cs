using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PayRoll.Models;

public class Teacher
{
    public int Id { get; set; }

    // Personal
    [Required(ErrorMessage = "First Name is required")]
    public string? FirstName { get; set; }

    public string? MiddleName { get; set; } // Not required

    [Required(ErrorMessage = "Last Name is required")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Date of Birth is required")]
    public DateTime? DateOfBirth { get; set; }

    // Professional
    [Required(ErrorMessage = "Employee ID is required")]
    public string? EmployeeId { get; set; }

    [Required(ErrorMessage = "Department is required")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "Subjects are required")]
    public string? Subjects { get; set; }

    [Required(ErrorMessage = "Designation is required")]
    public string? Designation { get; set; }

    [Required(ErrorMessage = "Date of Joining is required")]
    public DateTime? DateOfJoining { get; set; }

    // Account
    [Required(ErrorMessage = "Language Preference is required")]
    public string? LanguagePreference { get; set; }

    // File uploads (not mapped to DB)
    public string? PhotoPath { get; set; }
    public string? CVPath { get; set; }

    public int UserId { get; set; }
    public int AdminId { get; set; }
    public int SchoolId { get; set; }

    public User? User { get; set; }
    public Admin? Admin { get; set; }
    public School? School { get; set; }
}