namespace E_PayRoll.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameNepali { get; set; }

        public ICollection<Province>? Provinces { get; set; }
    }
}
