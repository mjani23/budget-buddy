using SQLite;

namespace FinanceFrenzy.Models
{
    public class MonthlyData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Data { get; set; } 
    }
}
