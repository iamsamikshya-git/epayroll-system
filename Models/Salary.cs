using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_PayRoll.Models
{
    public class Salary
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Employee/Teacher Name is required.")]
        [Display(Name = "Employee/Teacher Name")]
        public string TeacherId { get; set; }
        [MaxLength(200)]
        [BindNever]
        [Display(Name = "Teacher Name")]
        public string TeacherName { get; set; }

        [Required(ErrorMessage = "Appointment Type is required.")]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }

        [Required(ErrorMessage = "Level is required.")]
        public string Level { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Grade is required.")]
        public string Grade { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Basic Salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid basic salary.")]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid grade amount.")]
        [Display(Name = "Grade Amount")]
        public decimal GradeAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid employees' provident fund.")]
        [Display(Name = "Employees' Provident Fund")]
        public decimal EmployeesProvidentFund { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid citizen investment trust.")]
        [Display(Name = "Citizen Investment Trust")]
        public decimal CitizenInvestmentTrust { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid dearness allowance.")]
        [Display(Name = "Dearness Allowance")]
        public decimal DearnessAllowance { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid headmaster allowance.")]
        [Display(Name = "Headmaster Allowance")]
        public decimal HeadmasterAllowance { get; set; }

        [Display(Name = "Festival Allowance")]
        public bool FestivalAllowance { get; set; }

        [Display(Name = "Clothing Allowance")]
        public bool ClothingAllowance { get; set; }

        [Display(Name = "Total Allowance")]
        public decimal TotalAllowance { get; set; }

        [Display(Name = "Total Salary")]
        public decimal TotalSalary { get; set; }
        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending";   // "Pending" | "Accepted" | "Rejected"

    }
}
