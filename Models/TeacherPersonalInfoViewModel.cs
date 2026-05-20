using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;

namespace E_PayRoll.ViewModels
{
    public class TeacherPersonalInfoViewModel
    {
        // === Family Details ===
        public int Id { get; set; }

        [Required(ErrorMessage = "Father's name is required")]
        [Display(Name = "Father's Name")]
        public string FatherName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mother's name is required")]
        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; } = string.Empty;
        [Display(Name = "Spouse's Name")]

        public string? SpouseName { get; set; } 

        [Required(ErrorMessage = "Ethnicity is required")]
        public string Ethnicity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mother tongue is required")]
        [Display(Name = "Mother Tongue")]
        public string MotherTongue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Disability status is required")]
        public string Disability { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Contact { get; set; } = string.Empty;

        // === Document Information ===
        [Required(ErrorMessage = "Citizenship number is required")]
        [Display(Name = "Citizenship No")]
        public string CitizenshipNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Citizenship issue date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Citizenship Issue Date")]
        public DateTime CitizenshipIssueDate { get; set; }

        [Required(ErrorMessage = "Citizenship issue district is required")]
        [Display(Name = "Citizenship Issue District")]
        public string CitizenshipIssueDistrict { get; set; } = string.Empty;
        [Display(Name = "National ID No")]

        public string NationalId { get; set; } = string.Empty;
        [Display(Name = "Permanent Account No")]
        public string PermanentAccount { get; set; } = string.Empty;
        [Display(Name = "Employee Provident Fund No")]
        public string UnionFundNo { get; set; } = string.Empty;
        [Display(Name = "Citizen Investment Fund No")]
        public string InvestmentFundNo { get; set; } = string.Empty;
        [Display(Name = "Bank Name")]
        public string BankName { get; set; } = string.Empty;
        [Display(Name = "Bank Account No")]
        public string BankAccount { get; set; } = string.Empty;
        [Display(Name = "License No")]
        public string LicenseNo { get; set; } = string.Empty;
        [Display(Name = "Contribution Fund")]
        public string ContributionFund { get; set; } = string.Empty;
        [Display(Name = "Sheet Roll")]
        public string SheetRoll { get; set; } = string.Empty;

        // === Appointment Details ===
        [Required(ErrorMessage = "Teacher level is required")]
        [Display(Name = "Teacher Level")]
        public string TeacherLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teacher category is required")]
        [Display(Name = "Teacher Category")]
        public string TeacherCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment type is required")]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment status is required")]
        [Display(Name = "Appointment Status")]
        public string AppointmentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teacher status is required")]
        [Display(Name = "Teacher Status")]
        public string TeacherStatus { get; set; } = string.Empty;

        // Dropdown lists (marked as BindNever)
        [BindNever]
        public IEnumerable<SelectListItem> TeacherLevels { get; set; } = new List<SelectListItem>();

        [BindNever]
        public IEnumerable<SelectListItem> TeacherCategories { get; set; } = new List<SelectListItem>();

        [BindNever]
        public IEnumerable<SelectListItem> AppointmentTypes { get; set; } = new List<SelectListItem>();

        [BindNever]
        public IEnumerable<SelectListItem> DistrictList { get; set; } = new List<SelectListItem>();
    }
}