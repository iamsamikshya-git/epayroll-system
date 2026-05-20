using System;
using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{
    /// <summary>
    /// Row shown on the Municipality "Add" (decision) and "Salary List" pages.
    /// </summary>
    public class SalaryAcknowledgmentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Teacher Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Issued Date")]
        [DataType(DataType.Date)]
        public DateTime IssuedDate { get; set; }

        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Provident Fund")]
        public decimal PF { get; set; }

        [Display(Name = "CIT")]
        public decimal CIT { get; set; }

        [Display(Name = "Dearness Allowance")]
        public decimal Dearness { get; set; }

        [Display(Name = "Clothing Allowance")]
        public bool Clothing { get; set; }

        [Display(Name = "Festival Allowance")]
        public bool Festival { get; set; }
        [Display(Name = "Headmaster Allowance")]
        public decimal HeadmasterAllowance { get; set; } 

        [Display(Name = "Total Salary")]
        public decimal TotalSalary { get; set; }

        /// <summary>
        /// "Pending" | "Accepted" | "Rejected" (matches Salary.Status in DB)
        /// </summary>
        public string Status { get; set; } = "Pending";

        // Convenience for badge styling in Razor (optional)
        public string StatusBadgeClass =>
            Status == "Accepted" ? "bg-success" :
            Status == "Rejected" ? "bg-danger" :
            "bg-warning text-dark";
    }
}
