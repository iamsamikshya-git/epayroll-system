using System.ComponentModel.DataAnnotations;

namespace E_PayRoll.Models
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string SchoolName { get; set; } = string.Empty;

        [Required]
        public string Tole { get; set; } = string.Empty;

        [Required]
        public string TelephoneNo { get; set; } = string.Empty;

        [Required]
        public int WardNo { get; set; }

        [Required]
        public string AccountNo { get; set; } = string.Empty;

        [Required]
        public string ContactPerson { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserPerson { get; set; } = string.Empty;

        [Required]
        public string EMISCode { get; set; } = string.Empty;

        public int UserId { get; set; }
        public int AdminId { get; set; }

        public virtual User? User { get; set; }
        public virtual Admin? Admin { get; set; }
    }
}