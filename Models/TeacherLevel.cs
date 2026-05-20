using System.ComponentModel.DataAnnotations;
using E_PayRoll.Data;

namespace E_PayRoll.Models
{
    public class TeacherLevel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Level name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Level code is required")]
        public string Code { get; set; }
    }
}
