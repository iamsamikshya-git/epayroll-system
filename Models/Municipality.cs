namespace E_PayRoll.Models
{
    public class Municipality
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameNepali { get; set; }

        public int DistrictId { get; set; }
        public District? District { get; set; }
    }
}
