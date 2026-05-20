using System.ComponentModel.DataAnnotations;

using E_PayRoll.Data;

namespace E_PayRoll.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        // Personal Info
        //[Required]
        public int SchoolId { get; set; }

        public string SchoolName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        [Required]
        public string SchoolCode { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        public string MiddleName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; }

        // Permanent Address
        [Required(ErrorMessage = "Country is required")]
        public string PermanentCountry { get; set; } = string.Empty;
        [Required(ErrorMessage = "Province is required")]
        public string PermanentProvince { get; set; } = string.Empty;
        [Required(ErrorMessage = "District is required")]
        public string PermanentDistrict { get; set; } = string.Empty;
        [Required(ErrorMessage = "Local Level is required")]
        public string PermanentLocalLevel { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ward is required")]
        public string PermanentWard { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tole is required")]
        public string PermanentTole { get; set; } = string.Empty;

        // Temporary Address
        //public bool SamePermanent { get; set; }
       // [Required(ErrorMessage = "Temporary address is required")]
        public string TemporaryCountry { get; set; } = string.Empty;
        public string TemporaryProvince { get; set; } = string.Empty;
        public string TemporaryDistrict { get; set; } = string.Empty;
        public string TemporaryLocalLevel { get; set; } = string.Empty;
        public string TemporaryWard { get; set; } = string.Empty;
        public string TemporaryTole { get; set; } = string.Empty;
        //Family details
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string? SpouseName { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string MotherTongue { get; set; } = string.Empty;
        public string Disability { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Photo { get; set; }
        public string Contact { get; set; } = string.Empty;

        // Document Information

        public string CitizenshipNo { get; set; }
        
        [Display(Name = "Citizenship Issue Date")]
        [DataType(DataType.Date)]
        public DateTime? CitizenshipIssueDate { get; set; }

        [Display(Name = "Citizenship Issue District")]
        public string CitizenshipIssueDistrict { get; set; }

        [Display(Name = "National ID Number")]
        public string NationalId { get; set; }

        [Display(Name = "Permanent Account Number")]
        public string PermanentAccount { get; set; }

        [Display(Name = "Employee Union Fund No.")]
        public string UnionFundNo { get; set; }

        [Display(Name = "Citizen Investment Fund No.")]
        public string InvestmentFundNo { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Bank Account Number")]
        public string BankAccount { get; set; }

        [Display(Name = "Teaching License Number")]
        public string LicenseNo { get; set; }

        [Display(Name = "Highest Contribution-Based Grant Fund")]
        public string ContributionFund { get; set; }

        [Display(Name = "Sheet Roll")]
        public string SheetRoll { get; set; }

        // Appointment Details
        [Required]
        [Display(Name = "Teacher Level")]
        public string TeacherLevel { get; set; }

        [Required]
        [Display(Name = "Teacher Category")]
        public string TeacherCategory { get; set; }

        [Required]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }

        [Required]
        [Display(Name = "Appointment Status")]
        public string AppointmentStatus { get; set; }

        [Required]
        [Display(Name = "Teacher Status")]
        public string TeacherStatus { get; set; }

       
        // Qualification Fields
    [Required]
    [Display(Name = "Education Level")]
    public string EducationLevel { get; set; }

    [Required]
    [Display(Name = "Board / University")]
    public string BoardUniversity { get; set; }

    [Required]
    [Display(Name = "Passed Year")]
    public int PassedYear { get; set; }

    [Required]
    [Display(Name = "Faculty")]
    public string Faculty { get; set; }

    [Required]
    [Display(Name = "Grade / GPA")]
    public string GradeGPA { get; set; }

    // Experience Fields
    [Required]
    [Display(Name = "Post Type")]
    public string PostType { get; set; }

    [Required]
    [Display(Name = "Appointment Start Date")]
    public DateTime AppointmentStartDate { get; set; }

    [Required]
    [Display(Name = "Decision Date")]
    public DateTime DecisionDate { get; set; }

    [Required]
    [Display(Name = "Attendance Date")]
    public DateTime AttendanceDate { get; set; }

    [Required]
    [Display(Name = "Appointed School")]
    public string AppointedSchool { get; set; }

    [Required]
    [Display(Name = "School Address")]
    public string SchoolAddress { get; set; }

    [Required]
    [Display(Name = "Appointed Subject")]
    public string AppointedSubject { get; set; }
    }
}