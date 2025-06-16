using System.ComponentModel.DataAnnotations;
using E_PayRoll.Models;
namespace E_PayRoll.Models
{
    public class Admin
    {
        public int Id { get; set; }

        public string Email { get; set; } = "";

        public string? ProfilePicture { get; set; }

        public int CountryId { get; set; }
        public Country? Country { get; set; }

        public int ProvinceId { get; set; }
        public Province? Province { get; set; }

        public int DistrictId { get; set; }
        public District? District { get; set; }

        public int MunicipalityId { get; set; }
        public Municipality? Municipality { get; set; }

        // Foreign key reference to the user account
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
