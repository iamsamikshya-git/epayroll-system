using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{

    public class TeacherCategory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category Code")]
        public string Code { get; set; }
    }
}
