using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace E_PayRoll.ViewModels
{
    public class TeacherBasicInfoViewModel
    {
        // === Personal Info ===
      public int Id { get; set; }
      [Display(Name = "School Name")]
        public string SchoolName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

    [Required]
    [Display(Name = "School Code")]
        public string SchoolCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
        [Display (Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

    // === Permanent Address ===

    [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Permanent Country")]
        public string PermanentCountry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Province is required")]
        [Display(Name = "Permanent Province")]
        public string PermanentProvince { get; set; } = string.Empty;

    [Required(ErrorMessage = "District is required")]
        [Display(Name = "Permanent District")]
        public string PermanentDistrict { get; set; } = string.Empty;

    [Required(ErrorMessage = "Local Level is required")]
        [Display(Name = "Permanent Local Level")]
        public string PermanentLocalLevel { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ward is required")]
        [Display(Name = "Permanent Ward")]
        public string PermanentWard { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tole is required")]
        [Display(Name = "Permanent Tole")]
        public string PermanentTole { get; set; } = string.Empty;

        // === Temporary Address ===
[Display(Name = "Temporary Country")]
        public string TemporaryCountry { get; set; } = string.Empty;
        [Display(Name = "Temporary Province")]

        public string TemporaryProvince { get; set; } = string.Empty;
        [Display(Name = "Temporary District")]

        public string TemporaryDistrict { get; set; } = string.Empty;
        [Display(Name = "Temporary Local Level")]

        public string TemporaryLocalLevel { get; set; } = string.Empty;
        [Display(Name = "Temporary Ward")]

        public string TemporaryWard { get; set; } = string.Empty;
        [Display(Name = "Temporary Tole")]

        public string TemporaryTole { get; set; } = string.Empty;
    }
}
