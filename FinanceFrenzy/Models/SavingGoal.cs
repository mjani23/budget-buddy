
using SQLite;
using System;

namespace FinanceFrenzy.Models
{
    public class SavingGoal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double Percentage { get; set; }
    }
}
