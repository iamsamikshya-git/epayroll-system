using System;
using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.ViewModels
{
    public class TeacherExperienceViewModel
    {
        // === Appointment Details ===
         public int Id { get; set; }
        // [Required]
        // [Display(Name = "Teacher Level")]
        // public string TeacherLevel { get; set; }

        // [Required]
        // [Display(Name = "Teacher Category")]
        // public string TeacherCategory { get; set; }

        // [Required]
        // [Display(Name = "Appointment Type")]
        // public string AppointmentType { get; set; }

        // [Required]
        // [Display(Name = "Appointment Status")]
        // public string AppointmentStatus { get; set; }

        // [Required]
        // [Display(Name = "Teacher Status")]
        // public string TeacherStatus { get; set; }

        // === Qualification Fields ===

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

        // === Experience Fields ===

        [Required]
        [Display(Name = "Post Type")]
        public string PostType { get; set; }

        [Required]
        [Display(Name = "Appointment Start Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentStartDate { get; set; }

        [Required]
        [Display(Name = "Decision Date")]
        [DataType(DataType.Date)]
        public DateTime DecisionDate { get; set; }

        [Required]
        [Display(Name = "Attendance Date")]
        [DataType(DataType.Date)]
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
