public class AdminListViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? ProfilePicture { get; set; }

    public string CountryName { get; set; } = "";
    public string ProvinceName { get; set; } = "";
    public string DistrictName { get; set; } = "";
    public string MunicipalityName { get; set; } = "";
}
