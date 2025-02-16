using SQLite;
using System.IO;
using Microsoft.Maui.Storage;

namespace FinanceFrenzy.Models
{
    public static class DatabaseHelper
    {
        private static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FinanceFrenzy.db");

        public static void InitializeDatabase()
        {
            using (var db = new SQLiteConnection(dbPath))
            
            {
                db.CreateTable<UserInfo>();
                Console.WriteLine("------------------");
                Console.WriteLine($"Database Path: {dbPath}");
                

            }
        }

        public static void SaveIncomeData(string income)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = new UserInfo { Income = income };
                db.InsertOrReplace(userInfo);
            }
        }

        public static string LoadIncomeData()
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var userInfo = db.Table<UserInfo>().FirstOrDefault();
                return userInfo?.Income ?? "0";
            }
        }
    }
}
