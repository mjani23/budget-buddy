using SQLite;
using System;

namespace FinanceFrenzy.Models
{
    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Category { get; set; }
        public double Amount { get; set; }
        public string? Tag { get; set; } // Optional tag
        public DateTime Date { get; set; } // New date field

        public bool HasTag => !string.IsNullOrWhiteSpace(Tag);
    }
}
