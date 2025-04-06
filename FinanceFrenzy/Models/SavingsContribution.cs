using SQLite;

namespace FinanceFrenzy.Models
{
    public class Saving
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public string Tag { get; set; }

        public bool HasTag => !string.IsNullOrWhiteSpace(Tag);
    }
}
