using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class AdminCreateViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Country is required")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Province is required")]
    public int ProvinceId { get; set; }

    [Required(ErrorMessage = "District is required")]
    public int DistrictId { get; set; }

    [Required(ErrorMessage = "Municipality is required")]
    public int MunicipalityId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be 4-50 characters long")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;

    public IFormFile? ProfilePicture { get; set; }
    public string? ExistingProfilePicturePath { get; set; }
}
