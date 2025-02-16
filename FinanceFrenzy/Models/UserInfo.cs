using SQLite;

namespace FinanceFrenzy.Models
{
    public class UserInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Income { get; set; }
        public string State { get; set; }
        public string FilingStatus { get; set; }
        public double FederalTaxRate { get; set; }
        public double StateTaxRate { get; set; }
    }
}
