using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.ViewModels
{
    public class BasicSalaryViewModel
    {
        public int Id { get; set; }
       [Required]
        public int TeacherLevelId { get; set; }
        [Required]
        public int TeacherCategoryId { get; set; }
        [Required]
        
        public decimal BaseSalaryAmount { get; set; }

        public List<SelectListItem> Levels { get; set; }
        public List<SelectListItem> Categories { get; set; }
         
    }
}
