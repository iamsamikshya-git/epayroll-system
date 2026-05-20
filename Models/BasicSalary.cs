using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PayRoll.Models
{
    public class BasicSalary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Teacher Level")]
        public int TeacherLevelId { get; set; }

        [ForeignKey("TeacherLevelId")]
        public TeacherLevel TeacherLevel { get; set; }

        [Required]
        [Display(Name = "Teacher Category")]
        public int TeacherCategoryId { get; set; }

        [ForeignKey("TeacherCategoryId")]
        public TeacherCategory TeacherCategory { get; set; }
        
        [Required]
        [Display(Name = "Base Salary Amount")]
        public decimal BaseSalaryAmount { get; set; }
    }
}
