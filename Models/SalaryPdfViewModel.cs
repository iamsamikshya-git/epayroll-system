// Models/SalaryPdfViewModel.cs
namespace E_PayRoll.Models
{
    public class SalaryPdfViewModel
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }   // e.g., Municipality/ward
        public string TeacherName { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Status { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal GradeAmount { get; set; }
        public decimal PF { get; set; }     // Employees’ Provident Fund
        public decimal CIT { get; set; }
        public decimal Dearness { get; set; }
        public decimal HeadmasterAllowance { get; set; }
        public decimal ClothingAmount { get; set; }   // 0 if not applicable
        public decimal FestivalAmount { get; set; }   // 0 if not applicable

        public decimal TotalAllowance { get; set; }
        public decimal TotalSalary { get; set; }

        // Optional: for the letter header
        public string ReferenceNo { get; set; }
        public string FiscalYear { get; set; }
    }
}
