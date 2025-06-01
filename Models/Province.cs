namespace E_PayRoll.Models
{
    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
       

        public int CountryId { get; set; }
        public Country? Country { get; set; }

        public ICollection<District>? Districts { get; set; }
    }
}
