using SQLite;

namespace FinanceFrenzy.Models
{
    public class BudgetCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public bool CanBeDeleted { get; set; } = true; 
    }
}
