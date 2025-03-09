using SQLite;

namespace FinanceFrenzy.Models
{
    public class UserInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double Income { get; set; }
        public string? State { get; set; }
        public string? FilingStatus { get; set; }
        public double TakeHomePay { get; set; }
        

    }
}
