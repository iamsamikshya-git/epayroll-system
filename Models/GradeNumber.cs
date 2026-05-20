using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{
    public class GradeNumber
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Grade number is required")]
        [Display(Name = "Grade Number")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }
}
