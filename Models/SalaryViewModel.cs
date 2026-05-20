namespace E_PayRoll.Models
{
    public class SalaryViewModel
    {
        public int Id { get; set; }               // Unique salary record ID
        public string FullName { get; set; }      // Employee name
        public decimal BasicSalary { get; set; }  // Basic salary
        public decimal PF { get; set; }           // Provident Fund
        public decimal TotalSalary { get; set; }  // Total salary
        public string Status { get; set; }        // Current status (Pending, Approved, Rejected)
        public string Action { get; set; }  
             // Action by municipality (Accepted / Rejected)
    }
}
